using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaDerrota : MonoBehaviour
{
    EstadisticasJugador player;
    Image veil;
    CanvasGroup content;
    float shownAt;

    public static void Show(EstadisticasJugador stats)
    {
        var holder = new GameObject("PantallaDeDerrota");
        var screen = holder.AddComponent<PantallaDerrota>();
        screen.player = stats;
        screen.Build();
    }

    void Build()
    {
        shownAt = Time.unscaledTime;

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 600;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var events = new GameObject("EventSystem");
            events.AddComponent<UnityEngine.EventSystems.EventSystem>();
            events.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        var veilObject = new GameObject("Velo");
        veilObject.transform.SetParent(transform, false);
        veil = veilObject.AddComponent<Image>();
        veil.color = new Color(0.25f, 0f, 0f, 0f);
        Stretch(veil.rectTransform);

        var contentObject = new GameObject("Contenido");
        contentObject.transform.SetParent(transform, false);
        contentObject.AddComponent<RectTransform>();
        content = contentObject.AddComponent<CanvasGroup>();
        content.alpha = 0f;

        Label("HAS CAÍDO", 0, 130, 84, new Color(0.9f, 0.15f, 0.12f));
        Label("El umbral te reclamó, pero todavía no terminó contigo.", 0, 60, 26, new Color(1f, 1f, 1f, 0.8f));
        Button("Volver al último checkpoint", 0, -20, Retry);
        Button("Reiniciar el nivel", 0, -95, Restart);
        Button("Salir", 0, -170, Quit);
    }

    void Update()
    {
        float t = Mathf.Clamp01((Time.unscaledTime - shownAt) / 1.4f);
        if (veil != null) veil.color = new Color(0.25f, 0f, 0f, 0.9f * t);
        if (content != null) content.alpha = Mathf.Clamp01((t - 0.4f) / 0.6f);
    }

    void Retry()
    {
        Time.timeScale = 1f;
        player.RespawnAtCheckpoint();
        player.currentHP = player.maxHP * 0.6f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(gameObject);
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void Label(string text, float x, float y, int size, Color color)
    {
        var holder = new GameObject("Texto");
        holder.transform.SetParent(content.transform, false);
        var label = holder.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = text;
        label.fontSize = size;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = color;
        label.rectTransform.anchoredPosition = new Vector2(x, y);
        label.rectTransform.sizeDelta = new Vector2(1000, size + 30);
    }

    void Button(string text, float x, float y, UnityEngine.Events.UnityAction onClick)
    {
        var holder = new GameObject("Boton_" + text);
        holder.transform.SetParent(content.transform, false);
        var image = holder.AddComponent<Image>();
        image.color = new Color(0.12f, 0.05f, 0.05f, 0.95f);
        var button = holder.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        image.rectTransform.anchoredPosition = new Vector2(x, y);
        image.rectTransform.sizeDelta = new Vector2(380, 55);

        var caption = new GameObject("Titulo").AddComponent<Text>();
        caption.transform.SetParent(holder.transform, false);
        caption.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        caption.text = text;
        caption.fontSize = 26;
        caption.alignment = TextAnchor.MiddleCenter;
        caption.color = Color.white;
        caption.rectTransform.anchoredPosition = Vector2.zero;
        caption.rectTransform.sizeDelta = new Vector2(380, 55);
    }
}
