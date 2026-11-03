using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public float chaseRadius = 25f;
    public float attackRadius = 3.5f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    [Header("Referencias")]
    public Transform playerTransform;

    private NavMeshAgent agent;
    private PlayerStats playerStats;
    private float nextAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        BuscarJugador();
    }

    void BuscarJugador()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats != null) playerObj = playerStats.gameObject;
        }

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerStats = playerObj.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        if (playerTransform == null || playerStats == null)
        {
            BuscarJugador();
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= chaseRadius && distance > attackRadius)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
        }
        else if (distance <= attackRadius)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
            }

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void AttackPlayer()
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(attackDamage);
            Debug.Log($"[Boss] Golpeó al jugador. HP restante: {playerStats.currentHP}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}