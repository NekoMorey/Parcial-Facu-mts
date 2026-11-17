using UnityEngine;

public class Portal : MonoBehaviour
{
    public Vector3 destination;
    public string arrivalMessage = "Nivel 2  ·  El Abismo";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        TargetLock targetLock = stats.GetComponent<TargetLock>();
        if (targetLock != null) targetLock.UnlockTarget();

        PlayerController controller = stats.GetComponent<PlayerController>();
        if (controller != null) controller.ResetMotionState(destination);
        else stats.transform.position = destination;

        stats.SetCheckpoint(destination);
        stats.currentHP = stats.maxHP;
        stats.currentStamina = stats.maxStamina;

        GameFeedback.Play(GameFeedback.Cue.Reward);
        GameFeedback.Burst(destination, new Color(0.2f, 0.9f, 0.9f), 40, 0.25f);
        HUDManager.Notify(arrivalMessage, 4f);
    }
}
