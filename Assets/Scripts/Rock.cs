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
        GameFeedback.Play(GameFeedback.Cue.Impact);
        GameFeedback.Burst(collision.GetContact(0).point, new Color(0.78f, 0.55f, 0.28f), 10, 0.12f);
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