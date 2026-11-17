using UnityEngine;

public class Portal : MonoBehaviour
{
    public Vector3 destination;
    public string arrivalMessage = "Nivel 2  ·  El Abismo";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        EstadisticasJugador stats = other.GetComponent<EstadisticasJugador>();
        if (stats == null) stats = other.GetComponentInParent<EstadisticasJugador>();
        if (stats == null) return;

        FijarObjetivo targetLock = stats.GetComponent<FijarObjetivo>();
        if (targetLock != null) targetLock.UnlockTarget();

        ControlJugador controller = stats.GetComponent<ControlJugador>();
        if (controller != null) controller.ResetMotionState(destination);
        else stats.transform.position = destination;

        stats.SetCheckpoint(destination);
        stats.currentHP = stats.maxHP;
        stats.currentStamina = stats.maxStamina;

        EfectosJuego.Play(EfectosJuego.TipoSonido.Recompensa);
        EfectosJuego.Burst(destination, new Color(0.2f, 0.9f, 0.9f), 40, 0.25f);
        GestorHUD.Notify(arrivalMessage, 4f);
    }
}
