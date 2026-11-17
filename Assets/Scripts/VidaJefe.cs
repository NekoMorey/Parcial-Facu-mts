using UnityEngine;

public class VidaJefe : MonoBehaviour, IDanable
{
    [Header("Estadisticas del Jefe")]
    public string bossName = "Guardian del Umbral";
    public float maxHP = 500f;
    public float currentHP;
    public bool isFinalBoss = false;
    [HideInInspector] public bool engaged = false;

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
        EfectosJuego.Play(EfectosJuego.TipoSonido.JefeCae);
        Narracion.PlayVictoryLine();
        EfectosJuego.Burst(transform.position + Vector3.up, new Color(0.72f, 0.12f, 0.12f), 55, 0.32f);

        CofreBase[] chests = FindObjectsByType<CofreBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (CofreBase chest in chests)
        {
            chest.isBossDefeated = true;
        }

        if (isFinalBoss)
        {
            GestorHUD hud = FindFirstObjectByType<GestorHUD>();
            if (hud != null) hud.ShowVictory();
        }

        gameObject.SetActive(false);
    }
}
