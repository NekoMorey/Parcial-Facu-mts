using UnityEngine;

/// <summary>Centraliza la interacción y la recompensa de las variantes de cofre del proyecto.</summary>
public abstract class CofreBase : MonoBehaviour, IInteractuable
{
    public bool isBossDefeated;
    public float interactionDistance = 4.5f;

    [Header("Efectos de Subida de Nivel")]
    public GameObject levelUpVFX;
    public float scaleMultiplier = 1.25f;

    [Header("Siguiente nivel")]
    public GameObject portal;

    private bool isOpen;

    protected virtual void Update()
    {
        if (isOpen || !isBossDefeated) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest(player);
        }
    }

    public void Interact()
    {
        if (isOpen || !isBossDefeated) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) OpenChest(player);
    }

    private void OpenChest(GameObject player)
    {
        isOpen = true;
        EfectosJuego.Play(EfectosJuego.TipoSonido.Recompensa);
        EfectosJuego.Burst(transform.position + Vector3.up, new Color(1f, 0.72f, 0.2f), 35, 0.24f);
        transform.Rotate(0f, 45f, 0f);

        EstadisticasJugador stats = player.GetComponent<EstadisticasJugador>();
        if (stats != null) stats.RewardBossVictory();

        player.transform.localScale *= scaleMultiplier;

        if (levelUpVFX != null)
        {
            Instantiate(levelUpVFX, player.transform.position, Quaternion.identity, player.transform);
        }

        Debug.Log("Cofre abierto con exito! Nivel del personaje aumentado.");
        if (portal != null)
        {
            portal.SetActive(true);
            GestorHUD.Notify("Se abrió un portal hacia el Abismo", 4f);
            return;
        }

        GestorHUD hud = FindFirstObjectByType<GestorHUD>();
        if (hud != null) hud.ShowVictory();
    }
}