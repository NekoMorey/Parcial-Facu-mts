using UnityEngine;

public class TargetSwitch : MonoBehaviour
{
    public GameObject targetDoor;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            Debug.Log("Interruptor impactado por roca!");
            if (targetDoor != null)
            {
                Destroy(targetDoor);
            }
            Destroy(gameObject);
        }
    }
}
