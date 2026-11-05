using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Player HUD")]
    public PlayerStats playerStats;
    public Slider healthSlider;
    public Slider staminaSlider;

    [Header("Boss HUD")]
    public BossStats bossStats;
    public BossAI bossAI;
    public Slider bossHealthSlider;
    public GameObject bossHUDPanel;
    public TextMeshProUGUI bossNameText;

    private TextMeshProUGUI interactionText;
    private GameObject victoryPanel;
    private TMP_FontAsset hudFont;

    void Start()
    {
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (bossStats == null) bossStats = FindFirstObjectByType<BossStats>();
        if (bossAI == null) bossAI = FindFirstObjectByType<BossAI>();
        if (bossNameText == null && bossHUDPanel != null)
            bossNameText = bossHUDPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (bossNameText != null) hudFont = bossNameText.font;
        if (bossHUDPanel != null) bossHUDPanel.SetActive(false);
        CreateInteractionPrompt();
    }

    void Update()
    {
        if (playerStats != null)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = playerStats.maxHP;
                healthSlider.value = playerStats.currentHP;
            }
            if (staminaSlider != null)
            {
                staminaSlider.maxValue = playerStats.maxStamina;
                staminaSlider.value = playerStats.currentStamina;
            }
        }

        bool isBossActive = bossStats != null && bossStats.gameObject.activeInHierarchy;
        bool isDetected = bossAI != null && bossAI.isPlayerDetected;
        if (isBossActive && isDetected)
        {
            if (bossHUDPanel != null && !bossHUDPanel.activeSelf) bossHUDPanel.SetActive(true);
            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = bossStats.maxHP;
                bossHealthSlider.value = bossStats.currentHP;
            }
            if (bossNameText != null && !string.IsNullOrEmpty(bossStats.bossName))
                bossNameText.text = bossStats.bossName;
        }
        else if (bossHUDPanel != null && bossHUDPanel.activeSelf)
        {
            bossHUDPanel.SetActive(false);
        }

        UpdateInteractionPrompt();
    }

    private void CreateInteractionPrompt()
    {
        GameObject background = new GameObject("Interaction Prompt", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(transform, false);
        RectTransform rect = background.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 118f);
        rect.sizeDelta = new Vector2(520f, 54f);
        Image image = background.GetComponent<Image>();
        image.color = new Color(0.035f, 0.045f, 0.06f, 0.82f);
        image.raycastTarget = false;

        GameObject label = new GameObject("Prompt Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(background.transform, false);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 3f);
        labelRect.offsetMax = new Vector2(-12f, -3f);
        interactionText = label.GetComponent<TextMeshProUGUI>();
        interactionText.font = hudFont;
        interactionText.fontSize = 22f;
        interactionText.alignment = TextAlignmentOptions.Center;
        interactionText.color = Color.white;
        interactionText.raycastTarget = false;
        background.SetActive(false);
    }

    private void UpdateInteractionPrompt()
    {
        if (interactionText == null) return;
        string prompt = null;
        float nearest = float.PositiveInfinity;
        Vector3 playerPosition = playerStats != null ? playerStats.transform.position : Vector3.zero;

        ChestBase[] chests = FindObjectsByType<ChestBase>(FindObjectsSortMode.None);
        foreach (ChestBase chest in chests)
        {
            float distance = Vector3.Distance(playerPosition, chest.transform.position);
            if (distance <= chest.interactionDistance && distance < nearest)
            {
                nearest = distance;
                prompt = chest.isBossDefeated ? "E  ·  Abrir cofre" : "Derrota al Guardián para abrir el cofre";
            }
        }

        GameObject background = interactionText.transform.parent.gameObject;
        background.SetActive(!string.IsNullOrEmpty(prompt));
        if (!string.IsNullOrEmpty(prompt)) interactionText.text = prompt;
    }

    public void ShowVictory()
    {
        if (victoryPanel != null) return;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        victoryPanel = new GameObject("Victory Panel", typeof(RectTransform), typeof(Image));
        victoryPanel.transform.SetParent(transform, false);
        RectTransform panelRect = victoryPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        Image panelImage = victoryPanel.GetComponent<Image>();
        panelImage.color = new Color(0.025f, 0.035f, 0.045f, 0.9f);

        CreateVictoryLabel("VICTORIA", 48f, new Vector2(0f, 70f), new Vector2(900f, 100f), new Color(1f, 0.78f, 0.35f));
        CreateVictoryLabel("El Guardián del Umbral ha caído.\nLa recompensa es tuya.", 26f, new Vector2(0f, -12f), new Vector2(900f, 100f), Color.white);

        GameObject buttonObject = new GameObject("Restart Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(victoryPanel.transform, false);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -120f);
        buttonRect.sizeDelta = new Vector2(280f, 64f);
        buttonObject.GetComponent<Image>().color = new Color(0.78f, 0.48f, 0.15f, 1f);
        buttonObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (playerStats != null) playerStats.RestartGame();
        });
        CreateVictoryLabel("JUGAR DE NUEVO", 22f, new Vector2(0f, -120f), new Vector2(280f, 64f), new Color(0.08f, 0.06f, 0.04f));
    }

    private void CreateVictoryLabel(string value, float size, Vector2 position, Vector2 dimensions, Color color)
    {
        GameObject label = new GameObject("Victory Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(victoryPanel.transform, false);
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;
        TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
        text.font = hudFont;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.text = value;
        text.raycastTarget = false;
    }
}
