using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class DamageFeedback : MonoBehaviour
{
    static DamageFeedback instance;

    Image flash;
    float flashAlpha;
    float shakeStrength;
    Vector3 lastOffset;

    public static void Hit(float amount)
    {
        if (instance == null) Create();
        instance.flashAlpha = Mathf.Clamp(0.25f + amount * 0.015f, 0.25f, 0.6f);
        instance.shakeStrength = Mathf.Clamp(0.12f + amount * 0.01f, 0.12f, 0.45f);
    }

    public static void Shake(float strength)
    {
        if (instance == null) Create();
        instance.shakeStrength = Mathf.Max(instance.shakeStrength, strength);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        instance = null;
    }

    static void Create()
    {
        var holder = new GameObject("RetroalimentacionDeDanio");
        instance = holder.AddComponent<DamageFeedback>();
        instance.BuildOverlay();
    }

    void BuildOverlay()
    {
        var canvas = new GameObject("FlashCanvas").AddComponent<Canvas>();
        canvas.transform.SetParent(transform, false);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 400;

        var imageObject = new GameObject("Flash");
        imageObject.transform.SetParent(canvas.transform, false);
        flash = imageObject.AddComponent<Image>();
        flash.color = new Color(0.8f, 0.05f, 0.05f, 0f);
        flash.raycastTarget = false;
        flash.rectTransform.anchorMin = Vector2.zero;
        flash.rectTransform.anchorMax = Vector2.one;
        flash.rectTransform.offsetMin = Vector2.zero;
        flash.rectTransform.offsetMax = Vector2.zero;
    }

    void LateUpdate()
    {
        float step = Time.unscaledDeltaTime;

        if (flash != null)
        {
            flashAlpha = Mathf.MoveTowards(flashAlpha, 0f, step * 1.6f);
            flash.color = new Color(0.8f, 0.05f, 0.05f, flashAlpha);
        }

        var cam = Camera.main;
        if (cam == null) return;

        if (shakeStrength > 0.001f)
        {
            lastOffset = Random.insideUnitSphere * shakeStrength;
            cam.transform.position += lastOffset;
            shakeStrength = Mathf.MoveTowards(shakeStrength, 0f, step * 1.2f);
        }
    }
}
