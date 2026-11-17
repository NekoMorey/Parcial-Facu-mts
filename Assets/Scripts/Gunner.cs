using UnityEngine;

public class Gunner : MonoBehaviour, IDamageable
{
    public GameObject bulletPrefab;
    public float maxHP = 28f;
    public float sightRange = 26f;
    public float fireInterval = 1.9f;
    public float bulletDamage = 7f;
    public float bulletSpeed = 15f;
    public float strafeDistance = 3f;
    public float strafeSpeed = 1.2f;
    public float aimTurnSpeed = 6f;
    public float muzzleHeight = 1.4f;

    float currentHP;
    float fireTimer;
    float strafeClock;
    Vector3 home;
    Transform player;
    Renderer body;
    Color baseColor;
    float flash;

    void Start()
    {
        currentHP = maxHP;
        home = transform.position;
        fireTimer = Random.Range(0.5f, fireInterval);
        strafeClock = Random.Range(0f, 6f);
        body = GetComponentInChildren<Renderer>();
        if (body != null) baseColor = body.material.color;
    }

    void Update()
    {
        if (CinematicShot.Running) return;

        if (player == null)
        {
            var found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
        }

        if (flash > 0f)
        {
            flash -= Time.deltaTime;
            if (body != null) body.material.color = flash > 0f ? Color.white : baseColor;
        }

        Vector3 eye = transform.position + Vector3.up * muzzleHeight;
        Vector3 target = player.position + Vector3.up * 1f;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > sightRange || !HasLineOfSight(eye, target)) return;

        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.01f)
        {
            var wanted = Quaternion.LookRotation(flat);
            transform.rotation = Quaternion.Slerp(transform.rotation, wanted, aimTurnSpeed * Time.deltaTime);
        }

        strafeClock += Time.deltaTime * strafeSpeed;
        Vector3 side = Vector3.Cross(Vector3.up, flat.normalized);
        Vector3 wantedPosition = home + side * Mathf.Sin(strafeClock) * strafeDistance;
        wantedPosition.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, wantedPosition, 2.5f * Time.deltaTime);

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = fireInterval;
            Shoot(eye, target);
        }
    }

    bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        if (!Physics.Linecast(from, to, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)) return true;
        return hit.collider.GetComponentInParent<PlayerStats>() != null;
    }

    void Shoot(Vector3 from, Vector3 target)
    {
        if (bulletPrefab == null) return;
        Vector3 direction = (target - from).normalized;
        var shot = Instantiate(bulletPrefab, from + direction * 0.8f, Quaternion.LookRotation(direction));
        var bullet = shot.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.damage = bulletDamage;
            bullet.speed = bulletSpeed;
        }
        GameFeedback.Play(GameFeedback.Cue.Shoot);
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        flash = 0.12f;
        if (currentHP > 0f) return;

        GameFeedback.Burst(transform.position + Vector3.up, new Color(1f, 0.5f, 0.2f), 24, 0.2f);
        Destroy(gameObject);
    }
}
