using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TablillaHistoria : MonoBehaviour
{
    public static int found;
    public static int total;

    [TextArea] public string message = "Una tablilla antigua.";
    public float spinSpeed = 60f;
    public float bobHeight = 0.25f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetCounters()
    {
        found = 0;
        total = 0;
    }

    Vector3 home;
    bool taken;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        total++;
    }

    void Start()
    {
        home = transform.position;
    }

    void OnDestroy()
    {
        if (!taken) total = Mathf.Max(0, total - 1);
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = home + Vector3.up * Mathf.Sin(Time.time * 2f) * bobHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (taken || other.GetComponentInParent<ControlJugador>() == null) return;
        taken = true;
        found++;
        GestorHUD.Notify("Tablilla " + found + "/" + total + ": " + message, 5f);
        Destroy(gameObject);
    }
}
