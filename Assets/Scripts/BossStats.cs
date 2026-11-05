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
        Debug.Log($"{bossName} ha sido derrotado!" );
        GameFeedback.Play(GameFeedback.Cue.BossDown);
        GameFeedback.Burst(transform.position + Vector3.up, new Color(0.72f, 0.12f, 0.12f), 55, 0.32f);

        ChestBase[] chests = FindObjectsByType<ChestBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (ChestBase chest in chests)
        {
            chest.isBossDefeated = true;
        }

        gameObject.SetActive(false);
    }
}