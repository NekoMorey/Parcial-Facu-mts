using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isBossDefeated = false;
    private bool isOpen = false;
    public float interactionDistance = 4.5f;

    [Header("Efectos de Subida de Nivel")]
    public GameObject levelUpVFX;
    public float scaleMultiplier = 1.25f;

    void Update()
    {
        if (isOpen) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {
                if (isBossDefeated)
                {
                    OpenChest(player);
                }
            }
        }
    }

    public void Interact()
    {
        if (isOpen || !isBossDefeated) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            OpenChest(player);
        }
    }

    void OpenChest(GameObject player)
    {
        isOpen = true;
        transform.Rotate(0f, 45f, 0f);

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.RewardBossVictory();
        }

        player.transform.localScale *= scaleMultiplier;

        if (levelUpVFX != null)
        {
            Instantiate(levelUpVFX, player.transform.position, Quaternion.identity, player.transform);
        }

        Debug.Log("Cofre abierto con exito! Nivel del personaje aumentado.");
    }
}