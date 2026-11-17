using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmasJugador : MonoBehaviour
{
    public float meleeCooldown = 0.55f;
    public float meleeReach = 1.7f;
    public float meleeRadius = 1.6f;
    public float meleeStamina = 8f;
    public float damageMultiplier = 1.8f;
    public float hitDelay = 0.14f;

    private float nextSwing;
    private float aimStart = -10f;
    private Transform leftArm;
    private AnimacionJugador playerAnimation;
    private EstadisticasJugador stats;

    void Start()
    {
        playerAnimation = GetComponent<AnimacionJugador>();
        stats = GetComponent<EstadisticasJugador>();
        leftArm = transform.Find("Modelo/Cuerpo/Brazo_Izq");
    }

    public void Aim()
    {
        aimStart = Time.time;
    }

    void LateUpdate()
    {
        if (leftArm == null) return;
        float t = Time.time - aimStart;
        if (t < 0f || t > 0.42f) return;

        float raise = t < 0.08f ? t / 0.08f : (t < 0.28f ? 1f : 1f - (t - 0.28f) / 0.14f);
        leftArm.localRotation = Quaternion.Euler(-95f * Mathf.Clamp01(raise), 0f, 0f);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (!Input.GetMouseButtonDown(1)) return;
        if (Time.time < nextSwing) return;
        if (stats != null && !stats.UseStamina(meleeStamina)) return;

        nextSwing = Time.time + meleeCooldown;
        if (playerAnimation != null) playerAnimation.Slash();
        EfectosJuego.Play(EfectosJuego.TipoSonido.Disparo);
        StartCoroutine(Strike());
    }

    IEnumerator Strike()
    {
        yield return new WaitForSeconds(hitDelay);

        Collider body = GetComponent<Collider>();
        Vector3 origin = body != null ? body.bounds.center : transform.position + Vector3.up;
        Vector3 center = origin + transform.forward * meleeReach;
        float damage = (stats != null ? stats.baseStrength : 10f) * damageMultiplier;

        var struck = new HashSet<IDanable>();
        foreach (Collider hit in Physics.OverlapSphere(center, meleeRadius))
        {
            if (hit.transform.IsChildOf(transform)) continue;

            if (hit.CompareTag("Destructible"))
            {
                Destroy(hit.gameObject);
                continue;
            }

            IDanable target = hit.GetComponentInParent<IDanable>();
            if (target == null || !struck.Add(target)) continue;

            target.TakeDamage(damage);
            EfectosJuego.Play(EfectosJuego.TipoSonido.Impacto);
            EfectosJuego.Burst(hit.ClosestPoint(center), new Color(1f, 0.85f, 0.5f), 14, 0.14f);
        }

        if (struck.Count > 0) EfectoDanio.Shake(0.12f);
    }
}
