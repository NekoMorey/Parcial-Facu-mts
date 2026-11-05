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

    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (bossStats == null)
        {
            bossStats = FindFirstObjectByType<BossStats>();
        }

        if (bossAI == null)
        {
            bossAI = FindFirstObjectByType<BossAI>();
        }

        if (bossNameText == null && bossHUDPanel != null)
        {
            bossNameText = bossHUDPanel.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (bossHUDPanel != null)
        {
            bossHUDPanel.SetActive(false);
        }
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
            if (bossHUDPanel != null && !bossHUDPanel.activeSelf)
            {
                bossHUDPanel.SetActive(true);
            }

            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = bossStats.maxHP;
                bossHealthSlider.value = bossStats.currentHP;
            }

            if (bossNameText != null && !string.IsNullOrEmpty(bossStats.bossName))
            {
                bossNameText.text = bossStats.bossName;
            }
        }
        else
        {
            if (bossHUDPanel != null && bossHUDPanel.activeSelf)
            {
                bossHUDPanel.SetActive(false);
            }
        }
    }
}