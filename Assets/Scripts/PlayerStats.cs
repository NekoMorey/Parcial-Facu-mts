using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats Base")]
    public float maxHP = 100f;
    public float currentHP;
    public float maxStamina = 100f;
    public float currentStamina;
    public float baseStrength = 10f;
    public int bossesDefeated = 0;

    [Header("Regeneración de Estamina")]
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1.5f;
    private float regenTimer;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    void Update()
    {

        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            regenTimer = staminaRegenDelay;
            return true;
        }
        return false;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (currentHP <= 0)
        {
            Debug.Log("Jugador derrotado");
        }
    }

    public void RewardBossVictory()
    {
        bossesDefeated++;
        maxHP += 25f;           
        baseStrength += 5f;     
        maxStamina += 10f;     
        currentHP = maxHP;   
        currentStamina = maxStamina;
        Debug.Log($"¡Victoria contra el Boss #{bossesDefeated}! Stats aumentadas: MaxHP = {maxHP}, Fuerza = {baseStrength}");
    }
}