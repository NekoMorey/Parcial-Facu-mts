using UnityEngine;

public class Rock : MonoBehaviour
{
    public float lifeTime = 5f;
    private float damage = 10f;

    void Start()
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            damage = playerStats.baseStrength;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destructible") || collision.gameObject.name.Contains("Pared"))
        {
            Destroy(collision.gameObject); // Destruye la pared al impactar
            Destroy(gameObject);
            return;
        }

        BossStats boss = collision.gameObject.GetComponent<BossStats>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 0.1f);
    }
}