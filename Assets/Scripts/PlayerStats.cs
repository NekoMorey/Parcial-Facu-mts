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

    [Header("Estadísticas de Ataque y Nivel")]
    public int playerLevel = 1;
    public float baseStrength = 10f;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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
    }

    private void Die()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}