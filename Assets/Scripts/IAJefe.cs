using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IAJefe : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Rangos y Deteccion")]
    public float detectionRange = 38.0f;
    public float attackRange = 4.0f;
    public float chargeRange = 8.0f;
    public float timeBetweenAttacks = 1.0f;
    private float attackCooldown = 0f;

    [Header("Estadisticas de Ataque")]
    public float meleeDamage = 15f;
    public float chargeDamage = 25f;
    public float chargeSpeed = 16f;
    private float originalSpeed;
    private bool isAttacking = false;
    private bool dangerLinePlayed = false;

    public bool isPlayerDetected = false;

    [Header("Fase 2")]
    public float enragedThreshold = 0.5f;
    public float slamRadius = 6.5f;
    public float slamDamage = 20f;
    public float slamWindup = 0.9f;
    private bool enraged = false;
    private VidaJefe stats;
    private GameObject slamMarker;
    private CapsuleCollider bodyCollider;

    public float fallbackSpeed = 10f;
    private float manualSpeed;
    private float nextReport;
    private bool AgentReady => agent != null && agent.isOnNavMesh;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stats = GetComponent<VidaJefe>();
        bodyCollider = GetComponent<CapsuleCollider>();

        if (agent != null && !agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit cercano, 40f, NavMesh.AllAreas))
            agent.Warp(cercano.position);

        if (agent != null) originalSpeed = agent.speed;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        isPlayerDetected = false;
    }

    void Update()
    {
        KeepGrounded();

        if (player != null && Time.time >= nextReport)
        {
            nextReport = Time.time + 2f;
            Debug.Log("[Jefe] distancia " + Vector3.Distance(transform.position, player.position).ToString("F1") + " detectado " + isPlayerDetected + " navmesh " + AgentReady + " puerta " + PuertaBloqueando() + " cinematica " + TomaCinematica.Running + " ataque " + isAttacking + " pos " + transform.position);
        }

        if (animator != null && agent != null)
            animator.SetFloat("Velocidad", AgentReady ? agent.velocity.magnitude : manualSpeed);
        manualSpeed = 0f;

        if (stats != null) stats.engaged = isPlayerDetected;

        if (TomaCinematica.Running)
        {
            if (AgentReady) agent.ResetPath();
            attackCooldown = Mathf.Max(attackCooldown, 1f);
            return;
        }

        if (player == null || isAttacking) return;

        if (PuertaBloqueando())
        {
            if (AgentReady) agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        attackCooldown -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
            isPlayerDetected = true;

        if (!isPlayerDetected)
        {
            if (AgentReady) agent.isStopped = true;
            return;
        }

        if (!enraged && stats != null && stats.currentHP <= stats.maxHP * enragedThreshold)
            EnterPhaseTwo();

        if (!AgentReady && distanceToPlayer > attackRange)
        {
            if (distanceToPlayer >= chargeRange && attackCooldown <= 0f)
                StartCoroutine(ChargeRoutine());
            else
                MoveWithoutNavMesh();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            if (AgentReady) agent.isStopped = true;
            LookAtPlayer();

            if (attackCooldown <= 0f)
            {
                if (enraged && Random.value < 0.5f)
                    StartCoroutine(SlamRoutine());
                else
                    ExecuteMeleeAttack();
            }
        }
        else if (distanceToPlayer >= chargeRange && attackCooldown <= 0f)
        {
            StartCoroutine(ChargeRoutine());
        }
        else
        {
            if (AgentReady)
            {
                agent.isStopped = false;
                agent.speed = originalSpeed;
                agent.SetDestination(player.position);
            }
            else
            {
                MoveWithoutNavMesh();
            }
        }
    }

    void MoveWithoutNavMesh()
    {
        LookAtPlayer();
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        float speed = fallbackSpeed * (enraged ? 1.3f : 1f);
        transform.position += flat.normalized * speed * Time.deltaTime;
        manualSpeed = speed;
    }

    bool PuertaBloqueando()
    {
        foreach (PuertaCerrada puerta in PuertaCerrada.activas)
        {
            if (Vector3.Distance(puerta.transform.position, transform.position) <= detectionRange) return true;
        }
        return false;
    }

    void KeepGrounded()
    {
        if (bodyCollider == null) return;
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit floor, 20f, ~0, QueryTriggerInteraction.Ignore)) return;

        float halfHeight = bodyCollider.height * 0.5f * transform.lossyScale.y;
        float gap = transform.position.y - halfHeight - floor.point.y;
        if (Mathf.Abs(gap) < 0.03f) return;

        if (AgentReady)
            agent.baseOffset -= gap * 0.5f / transform.lossyScale.y;
        else
            transform.position -= Vector3.up * gap;
    }

    void EnterPhaseTwo()
    {
        enraged = true;
        originalSpeed *= 1.35f;
        chargeSpeed *= 1.2f;
        timeBetweenAttacks *= 0.65f;

        if (AgentReady) agent.speed = originalSpeed;

        Color fury = new Color(0.75f, 0.1f, 0.05f);
        foreach (Renderer piece in GetComponentsInChildren<Renderer>())
        {
            if (piece is ParticleSystemRenderer) continue;
            piece.material.color = Color.Lerp(piece.material.color, fury, 0.45f);
        }

        EfectosJuego.Play(EfectosJuego.TipoSonido.Embestida);
        EfectosJuego.Burst(transform.position + Vector3.up, new Color(1f, 0.3f, 0.1f), 60, 0.35f);
        GestorHUD.Notify("El Guardián entra en furia", 3f);
    }

    IEnumerator SlamRoutine()
    {
        isAttacking = true;
        attackCooldown = timeBetweenAttacks + 0.8f;

        if (AgentReady) agent.isStopped = true;
        if (animator != null) animator.SetTrigger("Aplastar");
        EfectosJuego.Play(EfectosJuego.TipoSonido.Embestida);

        Vector3 center = transform.position;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit ground, 12f, ~0, QueryTriggerInteraction.Ignore))
            center = ground.point;

        slamMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(slamMarker.GetComponent<Collider>());
        slamMarker.transform.position = center + Vector3.up * 0.03f;
        slamMarker.GetComponent<Renderer>().material.color = new Color(0.85f, 0.12f, 0.08f);

        float timer = 0f;
        while (timer < slamWindup)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(0.5f, slamRadius * 2f, timer / slamWindup);
            slamMarker.transform.localScale = new Vector3(size, 0.02f, size);
            yield return null;
        }

        Destroy(slamMarker);
        EfectosJuego.Play(EfectosJuego.TipoSonido.Impacto);
        EfectosJuego.Burst(center + Vector3.up * 0.5f, new Color(1f, 0.35f, 0.15f), 70, 0.3f);

        Vector3 toPlayer = player.position - center;
        toPlayer.y = 0f;
        if (toPlayer.magnitude <= slamRadius)
        {
            EstadisticasJugador playerStats = player.GetComponent<EstadisticasJugador>();
            if (playerStats != null) playerStats.TakeDamage(slamDamage);
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    void OnDisable()
    {
        if (slamMarker != null) Destroy(slamMarker);
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 12f);
    }

    void ExecuteMeleeAttack()
    {
        isAttacking = true;
        attackCooldown = timeBetweenAttacks;

        if (animator != null) animator.SetTrigger("Ataque");

        Invoke(nameof(ApplyMeleeDamage), 0.3f);
    }

    void ApplyMeleeDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 1.5f)
        {
            EstadisticasJugador stats = player.GetComponent<EstadisticasJugador>();
            if (stats != null) stats.TakeDamage(meleeDamage);
        }
        isAttacking = false;
    }

    IEnumerator ChargeRoutine()
    {
        isAttacking = true;
        attackCooldown = timeBetweenAttacks + 1.2f;

        if (animator != null) animator.SetTrigger("Embestida");
        EfectosJuego.Play(EfectosJuego.TipoSonido.Embestida);

        if (!dangerLinePlayed)
        {
            Narracion.PlayDangerLine();
            dangerLinePlayed = true;
        }

        Vector3 targetDirection = (player.position - transform.position).normalized;
        targetDirection.y = 0;

        if (AgentReady) agent.isStopped = true;
        if (targetDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(targetDirection);

        yield return new WaitForSeconds(0.3f);

        if (AgentReady)
        {
            agent.isStopped = false;
            agent.speed = chargeSpeed;
            agent.acceleration = 40f;
            agent.SetDestination(player.position + targetDirection * 4f);
        }

        float chargeTimer = 0f;
        bool hitPlayer = false;

        while (chargeTimer < 1.0f)
        {
            if (TomaCinematica.Running) break;
            chargeTimer += Time.deltaTime;

            if (!AgentReady)
            {
                transform.position += targetDirection * chargeSpeed * Time.deltaTime;
                manualSpeed = chargeSpeed;
            }

            if (!hitPlayer && Vector3.Distance(transform.position, player.position) <= 2.5f)
            {
                EstadisticasJugador stats = player.GetComponent<EstadisticasJugador>();
                if (stats != null) stats.TakeDamage(chargeDamage);
                hitPlayer = true;
            }

            yield return null;
        }

        if (AgentReady)
        {
            agent.speed = originalSpeed;
            agent.acceleration = 8f;
        }

        isAttacking = false;
    }
}
