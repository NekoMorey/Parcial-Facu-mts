using UnityEngine;

public class UpgradeChest : MonoBehaviour, IInteractable
{
    public enum Upgrade { Health, Stamina, Strength }

    public Upgrade upgrade = Upgrade.Health;
    public float amount = 25f;
    public float interactionDistance = 3.5f;
    public Transform lid;

    private bool isOpen;
    private Transform player;

    void Update()
    {
        if (isOpen) return;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
        }

        if (Vector3.Distance(transform.position, player.position) > interactionDistance) return;

        HUDManager.Notify("E  ·  Abrir cofre", 0.2f);
        if (Input.GetKeyDown(KeyCode.E)) Open();
    }

    public void Interact()
    {
        if (!isOpen && player != null) Open();
    }

    private void Open()
    {
        isOpen = true;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        string message = "";
        switch (upgrade)
        {
            case Upgrade.Health:
                stats.maxHP += amount;
                stats.currentHP = stats.maxHP;
                message = "+" + amount + " de vida máxima";
                break;

            case Upgrade.Stamina:
                stats.maxStamina += amount;
                stats.currentStamina = stats.maxStamina;
                message = "+" + amount + " de estamina máxima";
                break;

            case Upgrade.Strength:
                stats.baseStrength += amount;
                message = "+" + amount + " de fuerza";
                break;
        }

        if (lid != null) lid.localRotation = Quaternion.Euler(-70f, 0f, 0f);

        GameFeedback.Play(GameFeedback.Cue.Reward);
        GameFeedback.Burst(transform.position + Vector3.up, new Color(1f, 0.72f, 0.2f), 30, 0.22f);
        HUDManager.Notify(message, 3f);
    }
}
