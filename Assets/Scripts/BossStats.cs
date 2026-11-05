using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Stats del Boss")]
    public string bossName = "Guardian del Umbral";
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

        Debug.Log($"{bossName} recibio {amount} de dano. HP restante: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{bossName} ha sido derrotado!");

        Chest chest = FindFirstObjectByType<Chest>(FindObjectsInactive.Include);
        if (chest != null)
        {
            chest.isBossDefeated = true;
        }

        RewardChest rChest = FindFirstObjectByType<RewardChest>(FindObjectsInactive.Include);
        if (rChest != null)
        {
            rChest.isBossDefeated = true;
        }

        gameObject.SetActive(false);
    }
}