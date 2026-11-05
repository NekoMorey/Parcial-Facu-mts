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

    [Header("Estad�sticas de Ataque y Nivel")]
    public int playerLevel = 1;
    public float baseStrength = 10f;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;

    private Vector3 checkpointPosition;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        checkpointPosition = transform.position;

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
        GameFeedback.Play(GameFeedback.Cue.Hit);
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public void RespawnAtCheckpoint()
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.ResetMotionState(checkpointPosition);
        else transform.position = checkpointPosition;
        currentHP = Mathf.Max(currentHP, maxHP * 0.35f);
        currentStamina = maxStamina;
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
        GameFeedback.Burst(transform.position + Vector3.up, new Color(0.7f, 0.12f, 0.1f), 42, 0.28f);
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