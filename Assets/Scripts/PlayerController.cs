using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rb;
    private PlayerStats playerStats;
    private TargetLock targetLock;
    private Camera mainCamera;

    [Header("Movimiento")]
    public float moveSpeed = 15f;
    public float rotationSpeed = 12f;

    [Header("Target Lock / Fijar Enemigo")]
    public Transform targetEnemy;
    public bool isLockedOn = false;
    public float lockOnRange = 40f;

    [Header("Disparo / Ataque")]
    public GameObject rockPrefab;
    public Transform spawnPoint;
    public float rockForce = 25f;
    public float shootCooldown = 0.5f;
    private float lastShootTime = 0f;

    [Header("Salto")]
    public float jumpForce = 8.5f;
    public float gravity = -20f;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    [Header("Esquive / Dodge")]
    public float dodgeSpeed = 16f;
    public float dodgeDuration = 0.35f;
    public float dodgeStaminaCost = 25f;
    public float dodgeCooldown = 0.6f;

    private bool isDodging = false;
    private bool canDodge = true;
    [HideInInspector] public bool isInvulnerable = false;

    [Header("UI Sistema de Pausa")]
    public GameObject pauseMenuPanel;
    [HideInInspector] public bool isPaused = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        playerStats = GetComponent<PlayerStats>() ?? FindFirstObjectByType<PlayerStats>();
        targetLock = GetComponent<TargetLock>() ?? FindFirstObjectByType<TargetLock>();
        mainCamera = Camera.main;

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.center = Vector3.zero;
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.skinWidth = 0.05f;
            controller.minMoveDistance = 0f;
            controller.stepOffset = 0.4f;
        }

        if (controller != null)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        canDodge = true;
        isDodging = false;
        isInvulnerable = false;
        verticalVelocity = 0f;

        LockCursor();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (!isPaused && Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                LockCursor();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (isPaused || isDodging) return;

        CheckLockOnDistance();
        HandleGravityAndJump();
        HandleMovement();
        HandleShooting();
        HandleDodge();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }

    void CheckLockOnDistance()
    {
        if (targetLock != null)
        {
            isLockedOn = targetLock.IsLockedOn;
            targetEnemy = targetLock.currentTarget;
            return;
        }

        if (isLockedOn && targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            if (distance > lockOnRange)
            {
                isLockedOn = false;
                targetEnemy = null;
            }
        }
        else
        {
            isLockedOn = false;
            targetEnemy = null;
        }
    }

    public void ResetMotionState(Vector3 position)
    {
        StopAllCoroutines();
        if (controller != null && controller.enabled)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }
        else
        {
            transform.position = position;
        }
        if (rb != null) rb.linearVelocity = Vector3.zero;
        verticalVelocity = 0f;
        isDodging = false;
        canDodge = true;
        isInvulnerable = false;
    }
    void HandleGravityAndJump()
    {
        if (controller != null && controller.enabled)
        {
            float checkDist = 1.15f;
            isGrounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, checkDist);

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }

            verticalVelocity += gravity * Time.deltaTime;
            verticalVelocity = Mathf.Clamp(verticalVelocity, -25f, 25f);
        }
        else if (rb != null)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.15f);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 camForward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
        Vector3 camRight = mainCamera != null ? mainCamera.transform.right : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveZ + camRight * moveX;

        if (isLockedOn && targetEnemy != null)
        {
            Vector3 targetDir = (targetEnemy.position - transform.position).normalized;
            targetDir.y = 0f;
            if (targetDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else if (inputDir.magnitude >= 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        Vector3 horizontalMove = moveDir.normalized * moveSpeed;
        Vector3 finalMotion = new Vector3(horizontalMove.x, verticalVelocity, horizontalMove.z);

        if (controller != null && controller.enabled)
        {
            controller.Move(finalMotion * Time.deltaTime);
        }
        else if (rb != null)
        {
            if (moveDir.magnitude >= 0.1f)
            {
                Vector3 targetVel = moveDir.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
        else
        {
            if (moveDir.magnitude >= 0.1f)
                transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                ShootRock();
                lastShootTime = Time.time;
            }
        }
    }

    void ShootRock()
    {
        if (rockPrefab == null) return;

        Transform target = (isLockedOn && targetEnemy != null) ? targetEnemy : null;

        if (target != null)
        {
            Vector3 lookDir = (target.position - transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }

        Vector3 safeSpawnPos = spawnPoint != null
            ? spawnPoint.position
            : transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;

        Vector3 shootDirection;
        if (target != null)
        {
            Vector3 targetCenter = target.position + Vector3.up * 1.0f;
            shootDirection = (targetCenter - safeSpawnPos).normalized;
        }
        else
        {
            shootDirection = transform.forward;
        }

        Quaternion spawnRot = Quaternion.LookRotation(shootDirection);
        GameObject projectile = Instantiate(rockPrefab, safeSpawnPos, spawnRot);
        GameFeedback.Play(GameFeedback.Cue.Shoot);
        Rigidbody rockRb = projectile.GetComponent<Rigidbody>();

        if (rockRb != null)
        {
            rockRb.linearVelocity = Vector3.zero;
            rockRb.AddForce(shootDirection * rockForce, ForceMode.Impulse);
        }

        Rock rockComp = projectile.GetComponent<Rock>();
        if (rockComp != null)
            rockComp.Initialize(playerStats != null ? playerStats.baseStrength : 10f);
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        {
            if (playerStats == null || playerStats.UseStamina(dodgeStaminaCost))
            {
                float moveX = Input.GetAxisRaw("Horizontal");
                float moveZ = Input.GetAxisRaw("Vertical");
                StartCoroutine(DodgeRoutine(moveX, moveZ));
            }
        }
    }

    IEnumerator DodgeRoutine(float moveX, float moveZ)
    {
        isDodging = true;
        canDodge = false;
        isInvulnerable = true;
        GameFeedback.Play(GameFeedback.Cue.Dodge);
        GameFeedback.Burst(transform.position, new Color(0.45f, 0.78f, 1f), 12, 0.13f);

        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 camForward = mainCamera != null ? mainCamera.transform.forward : transform.forward;
        Vector3 camRight = mainCamera != null ? mainCamera.transform.right : transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 dodgeDirection;

        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f)
            dodgeDirection = (camForward * moveZ + camRight * moveX).normalized;
        else
            dodgeDirection = transform.forward;

        if (dodgeDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dodgeDirection);

        float actualDodgeSpeed = Mathf.Max(dodgeSpeed, moveSpeed * 1.5f);
        float timer = 0f;

        while (timer < dodgeDuration)
        {
            Vector3 movement = dodgeDirection * actualDodgeSpeed * Time.deltaTime;
            movement.y = verticalVelocity * Time.deltaTime;

            if (controller != null && controller.enabled)
            {
                controller.Move(movement);
            }
            else if (rb != null)
            {
                rb.linearVelocity = new Vector3(dodgeDirection.x * actualDodgeSpeed, rb.linearVelocity.y, dodgeDirection.z * actualDodgeSpeed);
            }
            else
            {
                transform.Translate(movement, Space.World);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isInvulnerable = false;
        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}