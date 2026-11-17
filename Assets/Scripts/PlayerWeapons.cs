using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public float meleeCooldown = 0.55f;
    public float meleeReach = 1.7f;
    public float meleeRadius = 1.6f;
    public float meleeStamina = 8f;
    public float damageMultiplier = 1.8f;
    public float hitDelay = 0.14f;

    private float nextSwing;
    private PlayerAnimation playerAnimation;
    private PlayerStats stats;

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (!Input.GetMouseButtonDown(1)) return;
        if (Time.time < nextSwing) return;
        if (stats != null && !stats.UseStamina(meleeStamina)) return;

        nextSwing = Time.time + meleeCooldown;
        if (playerAnimation != null) playerAnimation.Slash();
        GameFeedback.Play(GameFeedback.Cue.Shoot);
        StartCoroutine(Strike());
    }

    IEnumerator Strike()
    {
        yield return new WaitForSeconds(hitDelay);

        Collider body = GetComponent<Collider>();
        Vector3 origin = body != null ? body.bounds.center : transform.position + Vector3.up;
        Vector3 center = origin + transform.forward * meleeReach;
        float damage = (stats != null ? stats.baseStrength : 10f) * damageMultiplier;

        var struck = new HashSet<IDamageable>();
        foreach (Collider hit in Physics.OverlapSphere(center, meleeRadius))
        {
            if (hit.transform.IsChildOf(transform)) continue;

            if (hit.CompareTag("Destructible"))
            {
                Destroy(hit.gameObject);
                continue;
            }

            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null || !struck.Add(target)) continue;

            target.TakeDamage(damage);
            GameFeedback.Play(GameFeedback.Cue.Impact);
            GameFeedback.Burst(hit.ClosestPoint(center), new Color(1f, 0.85f, 0.5f), 14, 0.14f);
        }

        if (struck.Count > 0) DamageFeedback.Shake(0.12f);
    }
}
