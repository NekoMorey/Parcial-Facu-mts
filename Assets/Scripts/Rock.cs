using UnityEngine;

public class Rock : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 10f;

    public void Initialize(float rockDamage)
    {
        damage = rockDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
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