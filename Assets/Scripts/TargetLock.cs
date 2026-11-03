using UnityEngine;

public class TargetLock : MonoBehaviour
{
    [Header("Configuración de Lock-On")]
    public KeyCode lockKey = KeyCode.R;
    public float targetNoticeDistance = 20f;
    public LayerMask targetLayers;

    [Header("Referencias")]
    public Transform currentTarget;

    public bool IsLockedOn { get; private set; } = false;

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
            Debug.Log($"[Lock-On] Objetivo fijado: {currentTarget.name}");
        }
    }

    void UnlockTarget()
    {
        IsLockedOn = false;
        currentTarget = null;
        Debug.Log("[Lock-On] Objetivo liberado");
    }
}