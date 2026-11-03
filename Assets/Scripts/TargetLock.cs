using UnityEngine;
using Unity.Cinemachine;

public class TargetLock : MonoBehaviour
{
    [Header("Configuración de Lock-On")]
    public KeyCode lockKey = KeyCode.R;
    public float targetNoticeDistance = 25f;
    public LayerMask targetLayers;

    [Header("Referencias")]
    public CinemachineCamera cinemachineCam;
    public Transform currentTarget;

    public bool IsLockedOn { get; private set; } = false;

    private CinemachineOrbitalFollow orbitalFollow;

    void Start()
    {
        if (cinemachineCam != null)
        {
            orbitalFollow = cinemachineCam.GetComponent<CinemachineOrbitalFollow>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(lockKey))
        {
            if (IsLockedOn)
            {
                UnlockTarget();
            }
            else
            {
                LockOnClosestTarget();
            }
        }

        if (IsLockedOn && currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance > targetNoticeDistance || !currentTarget.gameObject.activeInHierarchy)
            {
                UnlockTarget();
            }
            else
            {
                UpdateOrbitalLockOn();
            }
        }
    }

    void UpdateOrbitalLockOn()
    {
        if (orbitalFollow == null || currentTarget == null) return;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0;

        if (dirToTarget != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
            orbitalFollow.HorizontalAxis.Value = Mathf.LerpAngle(orbitalFollow.HorizontalAxis.Value, targetAngle, Time.deltaTime * 10f);
        }
    }

    void LockOnClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetNoticeDistance, targetLayers);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Boss"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hit.transform;
                }
            }
        }

        if (closestTarget != null)
        {
            currentTarget = closestTarget;
            IsLockedOn = true;

            if (orbitalFollow == null && cinemachineCam != null)
            {
                orbitalFollow = cinemachineCam.GetComponent<CinemachineOrbitalFollow>();
            }
        }
    }

    public void UnlockTarget()
    {
        IsLockedOn = false;
        currentTarget = null;
    }
}