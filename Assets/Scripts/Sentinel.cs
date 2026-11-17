using UnityEngine;

public class Sentinel : MonoBehaviour, IDamageable
{
    private enum State { Patrol, Chase, Attack, Return }

    [Header("Vida")]
    public float maxHP = 30f;
    private float currentHP;

    [Header("Movimiento")]
    public float patrolRadius = 3f;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 6f;
    public float aggroRange = 12f;
    public float leashRange = 20f;

    [Header("Ataque")]
    public float attackRange = 2f;
    public float damage = 10f;
    public float windup = 0.4f;
    public float attackCooldown = 1.2f;

    private State state = State.Patrol;
    private Vector3 home;
    private Vector3 patrolPoint;
    private Transform player;
    private PlayerStats playerStats;
    private Renderer body;
    private Color baseColor;
    private float windupTimer;
    private float cooldownTimer;
    private float flashTimer;

    void Start()
    {
        currentHP = maxHP;
        home = transform.position;
        patrolPoint = home;

        body = GetComponentInChildren<Renderer>();
        if (body != null) baseColor = body.material.color;
    }

    void Update()
    {
        if (CinematicShot.Running) return;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
            playerStats = found.GetComponent<PlayerStats>();
        }

        cooldownTimer -= Time.deltaTime;
        UpdateColor();

        float toPlayer = Vector3.Distance(transform.position, player.position);
        float fromHome = Vector3.Distance(transform.position, home);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (toPlayer <= aggroRange) state = State.Chase;
                break;

            case State.Chase:
                MoveTowards(player.position + Vector3.up * 0.4f, chaseSpeed);
                if (fromHome > leashRange || toPlayer > aggroRange * 1.6f)
                    state = State.Return;
                else if (toPlayer <= attackRange && cooldownTimer <= 0f)
                {
                    windupTimer = windup;
                    state = State.Attack;
                }
                break;

            case State.Attack:
                windupTimer -= Time.deltaTime;
                if (windupTimer <= 0f)
                {
                    if (toPlayer <= attackRange + 0.8f && playerStats != null)
                        playerStats.TakeDamage(damage);

                    cooldownTimer = attackCooldown;
                    state = State.Chase;
                }
                break;

            case State.Return:
                MoveTowards(home, chaseSpeed);
                if (fromHome < 0.5f) state = State.Patrol;
                else if (toPlayer <= aggroRange * 0.6f && fromHome < leashRange * 0.5f) state = State.Chase;
                break;
        }
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoint) < 0.4f)
        {
            Vector2 offset = Random.insideUnitCircle * patrolRadius;
            patrolPoint = home + new Vector3(offset.x, 0f, offset.y);
        }

        if (!MoveTowards(patrolPoint, patrolSpeed))
            patrolPoint = home;
    }

    bool MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;
        if (distance < 0.05f) return true;

        direction /= distance;
        float step = Mathf.Min(speed * Time.deltaTime, distance);

        Vector3 flat = new Vector3(direction.x, 0f, direction.z);
        if (flat.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flat), Time.deltaTime * 8f);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, step + 0.7f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Player") && hit.collider.GetComponent<Rock>() == null)
                return false;
        }

        transform.position += direction * step;
        return true;
    }

    void UpdateColor()
    {
        if (body == null) return;

        flashTimer -= Time.deltaTime;
        if (flashTimer > 0f)
            body.material.color = Color.white;
        else if (state == State.Attack)
            body.material.color = new Color(1f, 0.25f, 0.1f);
        else
            body.material.color = baseColor;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        flashTimer = 0.12f;

        if (state == State.Patrol || state == State.Return)
            state = State.Chase;

        if (currentHP <= 0f)
        {
            GameFeedback.Play(GameFeedback.Cue.BossDown);
            GameFeedback.Burst(transform.position, new Color(0.55f, 0.25f, 0.9f), 30, 0.22f);
            Destroy(gameObject);
        }
    }
}
