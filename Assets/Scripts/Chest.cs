using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) return;
        isOpen = true;
        Debug.Log("Cofre abierto con éxito.");
        // Agregar lógica de animación / recompensa
        transform.Rotate(0, 45, 0); 
    }
}
