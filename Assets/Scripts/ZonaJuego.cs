using UnityEngine;
using UnityEngine.Events;

public sealed class ZonaJuego : MonoBehaviour
{
    public enum AccionZona { PuntoControl, Caida }
    public AccionZona action;
    public UnityEvent onTriggered = new UnityEvent();
    private bool fired;

    private void OnTriggerEnter(Collider other)
    {
        if (fired || !other.CompareTag("Player")) return;
        EstadisticasJugador stats = other.GetComponent<EstadisticasJugador>();
        if (stats == null) stats = other.GetComponentInParent<EstadisticasJugador>();
        if (stats == null) return;

        if (action == AccionZona.PuntoControl)
        {
            fired = true;
            stats.SetCheckpoint(transform.position + Vector3.up * 1.5f);
            EfectosJuego.Play(EfectosJuego.TipoSonido.Recompensa);
            Narracion.PlayRouteLine();
            EfectosJuego.Burst(transform.position + Vector3.up, new Color(0.25f, 0.85f, 0.82f), 28, 0.2f);
            Debug.Log("Checkpoint activado: final del parkour.");
        }
        else
        {
            stats.RespawnAtCheckpoint();
            EfectosJuego.Play(EfectosJuego.TipoSonido.Esquive);
            EfectosJuego.Burst(stats.transform.position, new Color(0.25f, 0.85f, 0.82f), 16, 0.16f);
        }
        onTriggered.Invoke();
    }
}

public static class InstaladorZonas
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (GameObject.Find("Zonas_Juego") != null) return;
        GameObject root = new GameObject("Zonas_Juego");
        CreateTrigger(root.transform, "Parkour Punto Control", new Vector3(0f, 7f, 43f), new Vector3(6f, 3f, 6f), ZonaJuego.AccionZona.PuntoControl);
        CreateTrigger(root.transform, "Parkour Reinicio Caida", new Vector3(0f, -4.8f, 27f), new Vector3(26f, 1.5f, 46f), ZonaJuego.AccionZona.Caida);
    }

    private static void CreateTrigger(Transform parent, string name, Vector3 position, Vector3 size, ZonaJuego.AccionZona action)
    {
        GameObject trigger = new GameObject(name);
        trigger.transform.SetParent(parent, false);
        trigger.transform.position = position;
        BoxCollider box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;
        Rigidbody body = trigger.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        ZonaJuego eventHandler = trigger.AddComponent<ZonaJuego>();
        eventHandler.action = action;
    }
}
