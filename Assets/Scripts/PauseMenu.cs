using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.P;

    GameObject panel;
    bool paused;

    void Start()
    {
        BuildPanel();
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) SetPaused(!paused);
    }

    void SetPaused(bool value)
    {
        paused = value;
        panel.SetActive(value);
        Time.timeScale = value ? 0f : 1f;
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    void BuildPanel()
    {
        var canvasObject = new GameObject("PauseCanvas");
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var events = new GameObject("EventSystem");
            events.AddComponent<UnityEngine.EventSystems.EventSystem>();
            events.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObject.transform, false);
        var back = panel.AddComponent<Image>();
        back.color = new Color(0f, 0f, 0f, 0.7f);
        Stretch(back.rectTransform);

        MakeLabel("PAUSA", 0, 140, 64);
        MakeButton("Continuar", 20, () => SetPaused(false));
        MakeButton("Reiniciar nivel", -50, () =>
        {
            SetPaused(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        MakeButton("Salir", -120, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        MakeLabel("Volumen", 0, -200, 28);
        MakeVolumeSlider(-240);
    }

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    Text MakeLabel(string text, float x, float y, int size)
    {
        var holder = new GameObject("Label_" + text);
        holder.transform.SetParent(panel.transform, false);
        var label = holder.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = text;
        label.fontSize = size;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.rectTransform.anchoredPosition = new Vector2(x, y);
        label.rectTransform.sizeDelta = new Vector2(500, size + 20);
        return label;
    }

    void MakeButton(string text, float y, UnityEngine.Events.UnityAction onClick)
    {
        var holder = new GameObject("Button_" + text);
        holder.transform.SetParent(panel.transform, false);
        var image = holder.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
        var button = holder.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        image.rectTransform.anchoredPosition = new Vector2(0, y);
        image.rectTransform.sizeDelta = new Vector2(300, 55);

        var caption = MakeLabel(text, 0, 0, 28);
        caption.transform.SetParent(holder.transform, false);
        caption.rectTransform.anchoredPosition = Vector2.zero;
        caption.rectTransform.sizeDelta = new Vector2(300, 55);
    }

    void MakeVolumeSlider(float y)
    {
        var holder = new GameObject("VolumeSlider");
        holder.transform.SetParent(panel.transform, false);
        var rect = holder.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, y);
        rect.sizeDelta = new Vector2(300, 30);

        var track = new GameObject("Track").AddComponent<Image>();
        track.transform.SetParent(holder.transform, false);
        track.color = new Color(1f, 1f, 1f, 0.3f);
        Stretch(track.rectTransform);

        var handle = new GameObject("Handle").AddComponent<Image>();
        handle.transform.SetParent(holder.transform, false);
        handle.color = Color.white;
        handle.rectTransform.sizeDelta = new Vector2(20, 30);

        var slider = holder.AddComponent<Slider>();
        slider.targetGraphic = handle;
        slider.handleRect = handle.rectTransform;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = AudioListener.volume;
        slider.onValueChanged.AddListener(value => AudioListener.volume = value);
    }
}
