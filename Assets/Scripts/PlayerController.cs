using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float strafeSpeedMultiplier = 0.75f;
    public float rotationSpeed = 15f;
    public Transform cameraTransform;

    [Header("Dodge / Roll")]
    public float dodgeSpeed = 16f;
    public float dodgeDuration = 0.3f;
    public float dodgeStaminaCost = 25f;
    private bool isDodging = false;

    [Header("Throwing")]
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float throwForce = 15f;

    private Rigidbody rb;
    private TargetLock targetLock;
    private PlayerStats playerStats;

    private Vector3 moveInput;
    private Vector3 currentDodgeDir;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        targetLock = GetComponent<TargetLock>();
        playerStats = GetComponent<PlayerStats>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        rb.freezeRotation = true;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0, v).normalized;

        // Orientación obligatoria al objetivo fijado (Lock-On)
        OrientToLockTarget();

        // Dodge / Esquive
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging)
        {
            if (playerStats != null && playerStats.UseStamina(dodgeStaminaCost))
            {
                StartCoroutine(PerformDodge());
            }
        }

        // Disparo de rocas
        if (Input.GetMouseButtonDown(0) && !isDodging)
        {
            ThrowRock();
        }

        // Interacción
        if (Input.GetKeyDown(KeyCode.E) && !isDodging)
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            rb.linearVelocity = currentDodgeDir * dodgeSpeed + new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            OrientToLockTarget();
            return;
        }

        if (targetLock != null && targetLock.IsLockedOn && targetLock.currentTarget != null)
        {
            // --- MOVIMIENTO MODO COMBATE (STRAFE) ---
            Vector3 moveDir = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.z;
            moveDir.y = 0;
            moveDir.Normalize();

            float speed = moveSpeed * strafeSpeedMultiplier;
            rb.linearVelocity = moveDir * speed + new Vector3(0, rb.linearVelocity.y, 0);

            OrientToLockTarget();
        }
        else
        {
            // --- MOVIMIENTO MODO LIBRE ---
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.linearVelocity = moveDir * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void OrientToLockTarget()
    {
        if (targetLock != null && targetLock.IsLockedOn && targetLock.currentTarget != null)
        {
            Vector3 dirToBoss = (targetLock.currentTarget.position - transform.position).normalized;
            dirToBoss.y = 0;
            if (dirToBoss != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dirToBoss);
            }
        }
    }

    IEnumerator PerformDodge()
    {
        isDodging = true;

        if (moveInput.magnitude >= 0.1f)
        {
            currentDodgeDir = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.z;
            currentDodgeDir.y = 0;
            currentDodgeDir.Normalize();
        }
        else
        {
            currentDodgeDir = transform.forward;
        }

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
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