using UnityEngine;

public class BalaEnemiga : MonoBehaviour
{
    public float speed = 14f;
    public float damage = 8f;
    public float lifeTime = 6f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.GetComponentInParent<JefePulpo>() != null) return;
        if (other.GetComponent<Proyectil>() != null) return;

        EstadisticasJugador player = other.GetComponentInParent<EstadisticasJugador>();
        if (player != null) player.TakeDamage(damage);

        EfectosJuego.Burst(transform.position, new Color(1f, 0.45f, 0.7f), 6, 0.1f);
        Destroy(gameObject);
    }
}
