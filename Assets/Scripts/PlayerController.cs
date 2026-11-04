using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rb;
    private PlayerStats playerStats;
    private Camera mainCamera;

    [Header("Movimiento")]
    public float moveSpeed = 6f;
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
        playerStats = GetComponent<PlayerStats>();
        mainCamera = Camera.main;

        LockCursor();

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        FindClosestBoss();
    }

    void Update()
    {
        if (!isPaused && Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused || isDodging) return;

        CheckLockOnDistance();
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

    void FindClosestBoss()
    {
        if (targetEnemy == null)
        {
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss != null) targetEnemy = boss.transform;
        }
    }

    void CheckLockOnDistance()
    {
        if (targetEnemy == null)
        {
            FindClosestBoss();
        }

        if (isLockedOn && targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            if (distance > lockOnRange)
            {
                isLockedOn = false;
            }
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

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

        if (moveDir.magnitude >= 0.1f)
        {
            if (controller != null)
            {
                controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            }
            else if (rb != null)
            {
                rb.MovePosition(transform.position + moveDir.normalized * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.World);
            }
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

        Transform target = targetEnemy;
        if (target == null)
        {
            GameObject bossObj = GameObject.FindWithTag("Boss");
            if (bossObj != null) target = bossObj.transform;
        }

        if (target != null)
        {
            Vector3 lookDir = (target.position - transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        Vector3 safeSpawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;

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
        Rigidbody rockRb = projectile.GetComponent<Rigidbody>();

        if (rockRb != null)
        {
            rockRb.linearVelocity = Vector3.zero;
            rockRb.AddForce(shootDirection * rockForce, ForceMode.Impulse);
        }
    }

    void HandleDodge()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift)) && canDodge)
        {
            if (playerStats != null && playerStats.UseStamina(dodgeStaminaCost))
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

        Vector3 camForward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
        Vector3 camRight = mainCamera != null ? mainCamera.transform.right : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 dodgeDirection;

        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f)
        {
            dodgeDirection = (camForward * moveZ + camRight * moveX).normalized;
        }
        else
        {
            dodgeDirection = transform.forward;
        }

        float timer = 0f;
        while (timer < dodgeDuration)
        {
            if (controller != null)
            {
                controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            }
            else if (rb != null)
            {
                rb.MovePosition(transform.position + dodgeDirection * dodgeSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(dodgeDirection * dodgeSpeed * Time.deltaTime, Space.World);
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