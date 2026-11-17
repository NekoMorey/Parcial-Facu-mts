using UnityEngine;

public class InteractorRayo : MonoBehaviour
{
    public float interactRange = 5f;
    public LayerMask interactableLayers = ~0;
    public KeyCode interactKey = KeyCode.E;
    public bool showDebugRay = true;

    private Camera mainCamera;
    private IInteractuable currentTarget;
    private RaycastHit lastHit;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    void DetectInteractable()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (showDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.cyan);

        if (Physics.Raycast(ray, out lastHit, interactRange, interactableLayers))
        {
            currentTarget = lastHit.collider.GetComponent<IInteractuable>()
                ?? lastHit.collider.GetComponentInParent<IInteractuable>();
        }
        else
        {
            currentTarget = null;
        }
    }

    public bool HasTarget => currentTarget != null;
    public string TargetName => lastHit.collider != null ? lastHit.collider.gameObject.name : string.Empty;

    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * interactRange);
    }
}
