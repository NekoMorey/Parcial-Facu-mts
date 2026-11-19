using UnityEngine;
using UnityEngine.SceneManagement;

public class EstadisticasJugador : MonoBehaviour
{
    [Header("Salud")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenPerSecond = 18f;
    public float staminaRegenDelay = 0.8f;
    private float staminaRegenTimer;

    [Header("Estad�sticas de Ataque y Nivel")]
    public int playerLevel = 1;
    public float baseStrength = 10f;

    [Header("Progreso")]
    public bool hasUmbralKey;

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

    void Update()
    {
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= Time.deltaTime;
            return;
        }

        if (currentStamina < maxStamina)
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenPerSecond * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        if (TomaCinematica.Running) return;

        ControlJugador controller = GetComponent<ControlJugador>();
        if (controller != null && controller.isInvulnerable)
        {
            return;
        }

        currentHP -= damage;
        EfectosJuego.Play(EfectosJuego.TipoSonido.Golpe);
        EfectoDanio.Hit(damage);
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    [ContextMenu("Probar derrota")]
    void ProbarDerrota()
    {
        TakeDamage(maxHP * 2f);
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public void RespawnAtCheckpoint()
    {
        ControlJugador controller = GetComponent<ControlJugador>();
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
            staminaRegenTimer = staminaRegenDelay;
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
        EfectosJuego.Burst(transform.position + Vector3.up, new Color(0.7f, 0.12f, 0.1f), 42, 0.28f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PantallaDerrota.Show(this);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}