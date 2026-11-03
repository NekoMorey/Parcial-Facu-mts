using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float strafeSpeedMultiplier = 0.7f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    [Header("Throwing")]
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float throwForce = 15f;

    private Rigidbody rb;
    private bool isGrounded;
    private TargetLock targetLock;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        targetLock = GetComponent<TargetLock>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (targetLock != null && targetLock.IsLockedOn && targetLock.currentTarget != null)
        {
            Vector3 dirToBoss = (targetLock.currentTarget.position - transform.position).normalized;
            dirToBoss.y = 0;
            if (dirToBoss != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToBoss), Time.deltaTime * 12f);
            }

            if (inputDir.magnitude >= 0.1f)
            {
                Vector3 moveDir = cameraTransform.right * h + cameraTransform.forward * v;
                moveDir.y = 0;
                float currentSpeed = moveSpeed * strafeSpeedMultiplier;
                rb.MovePosition(transform.position + moveDir.normalized * currentSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (inputDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);

                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                rb.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ThrowRock();
        }

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
            Vector3 throwDir = transform.forward + Vector3.up * 0.15f;
            rockRb.AddForce(throwDir.normalized * throwForce, ForceMode.Impulse);
        }
    }

    void TryInteract()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2.5f))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}