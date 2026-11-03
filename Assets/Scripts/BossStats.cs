using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Stats del Boss")]
    public string bossName = "Guardián del Umbral";
    public float maxHP = 500f;
    public float currentHP;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"{bossName} recibió {amount} de daño. HP restante: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"¡{bossName} ha sido derrotado!");

        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            player.RewardBossVictory();
        }

        gameObject.SetActive(false);
    }
}