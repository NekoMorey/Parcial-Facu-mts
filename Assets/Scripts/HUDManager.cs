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
    public Slider bossHealthSlider;
    public GameObject bossHUDPanel;

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


        if (bossStats != null && bossStats.gameObject.activeInHierarchy)
        {
            if (bossHUDPanel != null) bossHUDPanel.SetActive(true);
            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = bossStats.maxHP;
                bossHealthSlider.value = bossStats.currentHP;
            }
        }
        else
        {
            if (bossHUDPanel != null) bossHUDPanel.SetActive(false);
        }
    }
}