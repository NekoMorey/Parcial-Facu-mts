using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Salud")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 25f;

    [Header("Estadisticas de Ataque y Nivel")]
    public int playerLevel = 1;
    public float baseStrength = 10f;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;

    void Start()
    {
        if (maxHP <= 0f) maxHP = 100f;
        if (maxStamina <= 0f) maxStamina = 100f;

        currentHP = maxHP;
        currentStamina = maxStamina;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (currentHP > 0f && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    public void TakeDamage(float damage)
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null && controller.isInvulnerable)
        {
            return;
        }

        currentHP -= damage;
<<<<<<< Updated upstream
=======
        GameFeedback.Play(GameFeedback.Cue.Hit);
        DamageFeedback.Hit(damage);
>>>>>>> Stashed changes
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    public void RewardBossVictory()
    {
        playerLevel++;
        baseStrength += 15f;
        maxHP += 50f;
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    private void Die()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DefeatScreen.Show(this);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}