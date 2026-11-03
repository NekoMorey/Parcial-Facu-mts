using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider healthSlider;
    public Slider staminaSlider;

    void Update()
    {
        if (playerStats == null) return;

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
}