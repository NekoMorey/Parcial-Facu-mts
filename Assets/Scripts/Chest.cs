using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    private bool isPlayerInside = false;

    public void Interact()
    {
        if (isOpen) return;
        isOpen = true;
        Debug.Log("¡Cofre abierto con éxito!");
        transform.Rotate(0, 45, 0);
    }

    private void Update()
    {

        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}