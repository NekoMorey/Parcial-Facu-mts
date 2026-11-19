using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    static bool alreadyShown;

    GameObject home;
    GameObject controls;
    GameObject credits;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetFlag()
    {
        alreadyShown = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Spawn()
    {
        if (alreadyShown) return;
        alreadyShown = true;
        new GameObject("MenuPrincipal").AddComponent<MenuPrincipal>();
    }

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Build();
    }

    void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 700;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var events = new GameObject("EventSystem");
            events.AddComponent<UnityEngine.EventSystems.EventSystem>();
            events.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        var back = new GameObject("Fondo").AddComponent<Image>();
        back.transform.SetParent(transform, false);
        back.color = new Color(0.03f, 0.04f, 0.07f, 0.94f);
        Stretch(back.rectTransform);

        home = Panel("Inicio");
        Label(home.transform, "ECOS DEL UMBRAL", 0, 170, 78, new Color(0.85f, 0.9f, 1f));
        Label(home.transform, "Segundo Parcial", 0, 105, 28, new Color(0.6f, 0.75f, 0.95f));
        Button(home.transform, "Jugar", 10, Play);
        Button(home.transform, "Controles", -60, () => Switch(controls));
        Button(home.transform, "Créditos", -130, () => Switch(credits));
        Button(home.transform, "Salir", -200, Quit);

        controls = Panel("Controles");
        Label(controls.transform, "CONTROLES", 0, 190, 54, Color.white);
        Label(controls.transform,
            "WASD  Moverse\nEspacio  Saltar\nShift  Rodar (gasta estamina)\nClick izquierdo  Disparar pistola\nClick derecho  Golpe de daga\nR  Fijar objetivo\nE  Interactuar\nEsc / P  Pausa",
            0, 20, 30, new Color(1f, 1f, 1f, 0.9f), 520);
        Button(controls.transform, "Volver", -190, () => Switch(home));

        credits = Panel("Creditos");
        Label(credits.transform, "CRÉDITOS", 0, 190, 54, Color.white);
        Label(credits.transform,
            "Diseño y programación: Nicolas Gabriel Morey (NekoMorey)\nMotor: Unity 6\nMúsica y efectos generados por código",
            0, 40, 28, new Color(1f, 1f, 1f, 0.9f), 520);
        Button(credits.transform, "Volver", -190, () => Switch(home));

        Switch(home);
    }

    void Switch(GameObject target)
    {
        home.SetActive(target == home);
        controls.SetActive(target == controls);
        credits.SetActive(target == credits);
    }

    void Play()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject);
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    GameObject Panel(string label)
    {
        var panel = new GameObject(label);
        panel.transform.SetParent(transform, false);
        Stretch(panel.AddComponent<RectTransform>());
        return panel;
    }

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void Label(Transform parent, string text, float x, float y, int size, Color color, float height = 0f)
    {
        var holder = new GameObject("Texto");
        holder.transform.SetParent(parent, false);
        var label = holder.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = text;
        label.fontSize = size;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = color;
        label.rectTransform.anchoredPosition = new Vector2(x, y);
        label.rectTransform.sizeDelta = new Vector2(1100, height > 0f ? height : size + 30);
    }

    void Button(Transform parent, string text, float y, UnityEngine.Events.UnityAction onClick)
    {
        var holder = new GameObject("Boton_" + text);
        holder.transform.SetParent(parent, false);
        var image = holder.AddComponent<Image>();
        image.color = new Color(0.15f, 0.18f, 0.26f, 0.95f);
        var button = holder.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        image.rectTransform.anchoredPosition = new Vector2(0, y);
        image.rectTransform.sizeDelta = new Vector2(320, 55);

        var caption = new GameObject("Titulo").AddComponent<Text>();
        caption.transform.SetParent(holder.transform, false);
        caption.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        caption.text = text;
        caption.fontSize = 28;
        caption.alignment = TextAnchor.MiddleCenter;
        caption.color = Color.white;
        caption.rectTransform.anchoredPosition = Vector2.zero;
        caption.rectTransform.sizeDelta = new Vector2(320, 55);
    }
}
