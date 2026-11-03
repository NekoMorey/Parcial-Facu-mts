using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    [Header("Throwing")]
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float throwForce = 15f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Throw Rock Mechanics
        if (Input.GetMouseButtonDown(0))
        {
            ThrowRock();
        }

        // Raycast Interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void ThrowRock()
    {
        if (rockPrefab == null || throwPoint == null) return;
        GameObject rock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rockRb = rock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            Vector3 throwDir = cameraTransform.forward + Vector3.up * 0.2f;
            rockRb.AddForce(throwDir * throwForce, ForceMode.Impulse);
        }
    }

    void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 4f))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
