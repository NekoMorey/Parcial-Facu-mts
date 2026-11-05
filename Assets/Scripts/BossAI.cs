using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
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

    public bool isPlayerDetected = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        attackCooldown -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
        }

        if (!isPlayerDetected)
        {
            agent.isStopped = true;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            LookAtPlayer();

            if (attackCooldown <= 0f)
            {
                ExecuteMeleeAttack();
            }
        }
        else if (distanceToPlayer >= chargeRange && attackCooldown <= 0f)
        {
            StartCoroutine(ChargeRoutine());
        }
        else
        {
            agent.isStopped = false;
            agent.speed = originalSpeed;
            agent.SetDestination(player.position);
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 12f);
        }
    }

    void ExecuteMeleeAttack()
    {
        isAttacking = true;
        attackCooldown = timeBetweenAttacks;

        if (animator != null) animator.SetTrigger("Attack");
        // Damage feedback is played only if ApplyMeleeDamage actually connects.

        Invoke(nameof(ApplyMeleeDamage), 0.3f);
    }

    void ApplyMeleeDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 1.5f)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(meleeDamage);
            }
        }
        isAttacking = false;
    }

    IEnumerator ChargeRoutine()
    {
        isAttacking = true;
        attackCooldown = timeBetweenAttacks + 1.2f;
        if (animator != null) animator.SetTrigger("Charge");
        GameFeedback.Play(GameFeedback.Cue.Charge);

        agent.isStopped = true;
        Vector3 targetDirection = (player.position - transform.position).normalized;
        targetDirection.y = 0;
        if (targetDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }

        yield return new WaitForSeconds(0.3f);

        agent.isStopped = false;
        agent.speed = chargeSpeed;
        agent.acceleration = 40f;

        Vector3 chargeTarget = player.position + targetDirection * 4f;
        agent.SetDestination(chargeTarget);

        float chargeTimer = 0f;
        bool hitPlayer = false;

        while (chargeTimer < 1.0f)
        {
            chargeTimer += Time.deltaTime;

            if (!hitPlayer && Vector3.Distance(transform.position, player.position) <= 2.5f)
            {
                PlayerStats stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(chargeDamage);
                }
                hitPlayer = true;
            }

            yield return null;
        }

        agent.speed = originalSpeed;
        agent.acceleration = 8f;
        isAttacking = false;
    }

}
