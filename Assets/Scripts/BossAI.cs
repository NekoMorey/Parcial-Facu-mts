using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Rangos y Detección")]
    public float detectionRange = 15.0f;
    public float attackRange = 2.5f;
    public float chargeRange = 6.0f;
    public float timeBetweenAttacks = 0.8f;
    private float attackCooldown = 0f;

    [Header("Estadísticas de Ataque")]
    public float meleeDamage = 15f;
    public float chargeDamage = 25f;
    public float chargeSpeed = 16f;
    private float originalSpeed;
    private bool isAttacking = false;

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
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        attackCooldown -= Time.deltaTime;

        if (distanceToPlayer > detectionRange)
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

        Invoke(nameof(ApplyMeleeDamage), 0.3f);
    }

    void ApplyMeleeDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 1f)
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
        attackCooldown = timeBetweenAttacks + 1f;

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

            if (!hitPlayer && Vector3.Distance(transform.position, player.position) <= 2f)
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

    void Die()
    {
        RewardChest chest = FindFirstObjectByType<RewardChest>();
        if (chest != null)
        {
            chest.isBossDefeated = true;
        }

        Destroy(gameObject);
    }
}