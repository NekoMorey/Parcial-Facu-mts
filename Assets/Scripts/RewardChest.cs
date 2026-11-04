using UnityEngine;

public class RewardChest : MonoBehaviour
{
    public bool isBossDefeated = false;
    private bool isOpened = false;

    [Header("Efectos de Subida de Nivel")]
    public GameObject levelUpVFX;
    public float scaleMultiplier = 1.25f;

    private void OnTriggerStay(Collider other)
    {
        if (!isBossDefeated || isOpened) return;

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest(other.gameObject);
        }
    }

    void OpenChest(GameObject player)
    {
        isOpened = true;

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

        Debug.Log("¡Cofre abierto! Nivel del personaje aumentado.");
    }
}