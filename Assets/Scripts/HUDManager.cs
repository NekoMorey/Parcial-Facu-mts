using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si usás TextMeshPro

public class HUDManager : MonoBehaviour
{
    [Header("Player HUD")]
    public PlayerStats playerStats;
    public Slider healthSlider;
    public Slider staminaSlider;

    [Header("Boss HUD")]
    public BossStats bossStats;
    public Slider bossHealthSlider;
    public GameObject bossHUDPanel; // Contenedor para ocultarlo si no hay Boss activo

    void Update()
    {
        // Actualizar Player
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