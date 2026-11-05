using UnityEngine;
using UnityEngine.Events;

public sealed class GameplayTrigger : MonoBehaviour
{
    public enum TriggerAction { Checkpoint, FallReset }
    public TriggerAction action;
    public UnityEvent onTriggered = new UnityEvent();
    private bool fired;

    private void OnTriggerEnter(Collider other)
    {
        if (fired || !other.CompareTag("Player")) return;
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        fired = true;
        if (action == TriggerAction.Checkpoint)
        {
            stats.SetCheckpoint(transform.position + Vector3.up * 1.5f);
            GameFeedback.Play(GameFeedback.Cue.Reward);
            GameFeedback.Burst(transform.position + Vector3.up, new Color(0.25f, 0.85f, 0.82f), 28, 0.2f);
            Debug.Log("Checkpoint activado: final del parkour.");
        }
        else
        {
            stats.RespawnAtCheckpoint();
            GameFeedback.Play(GameFeedback.Cue.Dodge);
            GameFeedback.Burst(stats.transform.position, new Color(0.25f, 0.85f, 0.82f), 16, 0.16f);
        }
        onTriggered.Invoke();
    }
}

public static class GameplayTriggerInstaller
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (GameObject.Find("VS_GameplayTriggers") != null) return;
        GameObject root = new GameObject("VS_GameplayTriggers");
        CreateTrigger(root.transform, "Parkour Checkpoint", new Vector3(0f, 7f, 43f), new Vector3(6f, 3f, 6f), GameplayTrigger.TriggerAction.Checkpoint);
        CreateTrigger(root.transform, "Parkour Fall Reset", new Vector3(0f, -4.8f, 27f), new Vector3(26f, 1.5f, 46f), GameplayTrigger.TriggerAction.FallReset);
    }

    private static void CreateTrigger(Transform parent, string name, Vector3 position, Vector3 size, GameplayTrigger.TriggerAction action)
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
        GameplayTrigger eventHandler = trigger.AddComponent<GameplayTrigger>();
        eventHandler.action = action;
    }
}
