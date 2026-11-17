using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBoss : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject bulletPrefab;
    public Transform[] muzzles;
    public Transform[] tentacles;
    public Transform[] pools;

    [Header("Combate")]
    public float activationRange = 26f;
    public float bulletSpeed = 14f;
    public float bulletDamage = 8f;
    public float restTime = 1.4f;
    public float turnSpeed = 4f;

    [Header("Morteros")]
    public float mortarRadius = 2.6f;
    public float mortarDamage = 18f;
    public float mortarDelay = 1f;

    private Transform player;
    private PlayerStats playerStats;
    private BossStats stats;
    private Quaternion[] tentacleRest;
    private readonly List<GameObject> markers = new List<GameObject>();
    private bool fighting;
    private bool enraged;
    private bool spinning;
    private int pattern;
    private int currentPool;
    private float surfaceY;

    void Start()
    {
        stats = GetComponent<BossStats>();
        surfaceY = transform.position.y;

        tentacleRest = new Quaternion[tentacles.Length];
        for (int i = 0; i < tentacles.Length; i++)
            tentacleRest[i] = tentacles[i].localRotation;
    }

    void Update()
    {
        if (CinematicShot.Running) return;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
            playerStats = found.GetComponent<PlayerStats>();
        }

        SwayTentacles();

        if (spinning)
        {
            transform.Rotate(0f, 150f * Time.deltaTime, 0f);
        }
        else
        {
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * turnSpeed);
        }

        if (!fighting && Vector3.Distance(transform.position, player.position) <= activationRange)
        {
            fighting = true;
            if (stats != null) stats.engaged = true;
            HUDManager.Notify("El Kraken Pistolero desenfunda", 3f);
            StartCoroutine(Fight());
        }
    }

    void SwayTentacles()
    {
        for (int i = 0; i < tentacles.Length; i++)
        {
            float pitch = Mathf.Sin(Time.time * 1.6f + i * 0.9f) * 12f;
            float roll = Mathf.Cos(Time.time * 1.2f + i * 0.6f) * 9f;
            tentacles[i].localRotation = tentacleRest[i] * Quaternion.Euler(pitch, 0f, roll);
        }
    }

    IEnumerator Fight()
    {
        yield return new WaitForSeconds(1.2f);

        while (true)
        {
            if (!enraged && stats != null && stats.currentHP <= stats.maxHP * 0.5f)
            {
                Enrage();
                yield return StartCoroutine(Relocate());
            }

            switch (pattern % (enraged ? 4 : 3))
            {
                case 0: yield return StartCoroutine(Burst()); break;
                case 1: yield return StartCoroutine(Fan()); break;
                case 2: yield return StartCoroutine(Spiral()); break;
                case 3: yield return StartCoroutine(Mortars()); break;
            }

            pattern++;

            if (enraged && pattern % 2 == 0)
                yield return StartCoroutine(Relocate());

            yield return new WaitForSeconds(enraged ? restTime * 0.55f : restTime);
        }
    }

    void Enrage()
    {
        enraged = true;

        Color fury = new Color(0.9f, 0.15f, 0.2f);
        foreach (Renderer piece in GetComponentsInChildren<Renderer>())
        {
            if (piece is ParticleSystemRenderer) continue;
            piece.material.color = Color.Lerp(piece.material.color, fury, 0.35f);
        }

        GameFeedback.Play(GameFeedback.Cue.Charge);
        GameFeedback.Burst(transform.position + Vector3.up * 3f, new Color(1f, 0.3f, 0.5f), 70, 0.35f);
        HUDManager.Notify("El Kraken se sumerge y cambia de posición", 3f);
    }

    void Shoot(Vector3 origin, Vector3 direction, float speed)
    {
        if (bulletPrefab == null || direction.sqrMagnitude < 0.001f) return;

        GameObject shot = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction));
        EnemyBullet bullet = shot.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.speed = speed;
            bullet.damage = bulletDamage;
        }
    }

    Vector3 ChestHeight()
    {
        return new Vector3(transform.position.x, surfaceY + 1f, transform.position.z);
    }

    IEnumerator Burst()
    {
        int volleys = enraged ? 4 : 3;

        for (int v = 0; v < volleys; v++)
        {
            foreach (Transform muzzle in muzzles)
            {
                Shoot(muzzle.position, player.position - muzzle.position, bulletSpeed);
                GameFeedback.Play(GameFeedback.Cue.Shoot);
                yield return new WaitForSeconds(0.12f);
            }

            yield return new WaitForSeconds(0.45f);
        }
    }

    IEnumerator Fan()
    {
        int waves = enraged ? 3 : 2;
        int count = enraged ? 11 : 7;

        for (int w = 0; w < waves; w++)
        {
            Vector3 origin = ChestHeight();
            Vector3 aim = player.position - origin;
            aim.y = 0f;
            aim.Normalize();

            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-45f, 45f, i / (count - 1f));
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * aim;
                Shoot(origin + direction * 3f, direction, bulletSpeed * 0.85f);
            }

            GameFeedback.Play(GameFeedback.Cue.Shoot);
            yield return new WaitForSeconds(0.6f);
        }
    }

    IEnumerator Spiral()
    {
        spinning = true;
        int arms = enraged ? 3 : 2;
        float duration = enraged ? 4.2f : 3f;
        float timer = 0f;
        float angle = 0f;

        while (timer < duration)
        {
            Vector3 origin = ChestHeight();
            for (int a = 0; a < arms; a++)
            {
                Vector3 direction = Quaternion.Euler(0f, angle + a * 360f / arms, 0f) * Vector3.forward;
                Shoot(origin + direction * 3f, direction, bulletSpeed * 0.6f);
            }

            angle += 17f;
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        spinning = false;
    }

    IEnumerator Mortars()
    {
        Vector3 floor = player.position;
        floor.y = surfaceY + 0.03f;

        Vector3[] targets =
        {
            floor,
            floor + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            floor + new Vector3(Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f)),
            floor + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f))
        };

        foreach (Vector3 target in targets)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(marker.GetComponent<Collider>());
            marker.transform.position = target;
            marker.transform.localScale = new Vector3(0.4f, 0.02f, 0.4f);
            marker.GetComponent<Renderer>().material.color = new Color(1f, 0.25f, 0.5f);
            markers.Add(marker);
        }

        GameFeedback.Play(GameFeedback.Cue.Charge);

        float timer = 0f;
        while (timer < mortarDelay)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(0.4f, mortarRadius * 2f, timer / mortarDelay);
            foreach (GameObject marker in markers)
                marker.transform.localScale = new Vector3(size, 0.02f, size);
            yield return null;
        }

        bool hit = false;
        foreach (Vector3 target in targets)
        {
            GameFeedback.Burst(target + Vector3.up * 0.4f, new Color(1f, 0.4f, 0.6f), 30, 0.25f);

            Vector3 offset = player.position - target;
            offset.y = 0f;
            if (offset.magnitude <= mortarRadius) hit = true;
        }

        GameFeedback.Play(GameFeedback.Cue.Impact);
        if (hit && playerStats != null) playerStats.TakeDamage(mortarDamage);

        ClearMarkers();
    }

    IEnumerator Relocate()
    {
        if (pools == null || pools.Length < 2) yield break;

        spinning = false;
        GameFeedback.Play(GameFeedback.Cue.Dodge);
        yield return StartCoroutine(Dive(surfaceY, surfaceY - 8f));

        int next = (currentPool + Random.Range(1, pools.Length)) % pools.Length;
        currentPool = next;
        transform.position = new Vector3(pools[next].position.x, surfaceY - 8f, pools[next].position.z);

        yield return StartCoroutine(Dive(surfaceY - 8f, surfaceY));
        GameFeedback.Burst(transform.position + Vector3.up, new Color(0.3f, 0.8f, 0.9f), 40, 0.3f);
    }

    IEnumerator Dive(float from, float to)
    {
        float timer = 0f;
        while (timer < 0.7f)
        {
            timer += Time.deltaTime;
            Vector3 position = transform.position;
            position.y = Mathf.Lerp(from, to, timer / 0.7f);
            transform.position = position;
            yield return null;
        }
    }

    void ClearMarkers()
    {
        foreach (GameObject marker in markers)
            if (marker != null) Destroy(marker);
        markers.Clear();
    }

    void OnDisable()
    {
        ClearMarkers();
    }
}
