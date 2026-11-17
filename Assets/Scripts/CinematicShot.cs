using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CinematicShot : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 cameraOffset = new Vector3(0f, 6f, -14f);
    public float duration = 3.5f;
    public float drift = 2f;
    public string caption;
    public bool playOnce = true;

    public static bool Running;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState()
    {
        Running = false;
    }

    bool played;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnDisable()
    {
        Running = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (played && playOnce) return;
        var player = other.GetComponentInParent<PlayerController>();
        if (player == null || lookAt == null) return;
        played = true;
        StartCoroutine(Play(player));
    }

    IEnumerator Play(PlayerController player)
    {
        var shotObject = new GameObject("CamaraCinematica");
        var shot = shotObject.AddComponent<CinemachineCamera>();
        shot.Priority.Value = 100;
        shotObject.transform.position = lookAt.position + cameraOffset;
        shotObject.transform.rotation = Quaternion.LookRotation(lookAt.position - shotObject.transform.position);

        Running = true;
        player.enabled = false;
        if (!string.IsNullOrEmpty(caption)) HUDManager.Notify(caption, duration);

        float elapsed = 0f;
        Vector3 start = shotObject.transform.position;
        Vector3 end = start + shotObject.transform.right * drift;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            shotObject.transform.position = Vector3.Lerp(start, end, t);
            Vector3 direction = lookAt.position - shotObject.transform.position;
            if (direction.sqrMagnitude > 0.01f)
                shotObject.transform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }

        Running = false;
        if (player != null) player.enabled = true;
        Destroy(shotObject);
    }
}
