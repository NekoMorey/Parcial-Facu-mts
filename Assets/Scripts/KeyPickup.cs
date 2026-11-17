using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public float spinSpeed = 90f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * 2f) * bobHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        stats.hasUmbralKey = true;
        GameFeedback.Play(GameFeedback.Cue.Reward);
        GameFeedback.Burst(transform.position, new Color(1f, 0.75f, 0.25f), 30, 0.2f);
        HUDManager.Notify("Conseguiste la llave del Umbral", 3.5f);
        Destroy(gameObject);
    }
}
