using UnityEngine;

public class Rock : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5f); // Auto destruir después de 5 segs
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
