using UnityEngine;

public class Proyectil : MonoBehaviour
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

        GameObject player = GameObject.FindWithTag("Player");
        Collider own = GetComponent<Collider>();
        if (player == null || own == null) return;
        foreach (Collider other in player.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(own, other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        EfectosJuego.Play(EfectosJuego.TipoSonido.Impacto);
        EfectosJuego.Burst(collision.GetContact(0).point, new Color(0.78f, 0.55f, 0.28f), 10, 0.12f);
        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        IDanable target = collision.gameObject.GetComponentInParent<IDanable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 0.1f);
    }
}