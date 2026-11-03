using UnityEngine;
using Unity.Cinemachine;

public class TargetLock : MonoBehaviour
{
    [Header("Configuración de Lock-On")]
    public KeyCode lockKey = KeyCode.R;
    public float targetNoticeDistance = 20f;
    public LayerMask targetLayers;

    [Header("Referencias")]
    public CinemachineCamera cinemachineCam;
    public Transform currentTarget;

    private bool isLockedOn = false;

    void Update()
    {
        if (Input.GetKeyDown(lockKey))
        {
            if (isLockedOn)
            {
                UnlockTarget();
            }
            else
            {
                LockOnClosestTarget();
            }
        }

        if (isLockedOn && currentTarget != null)
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
            isLockedOn = true;

         
            if (cinemachineCam != null)
            {
                cinemachineCam.LookAt = currentTarget;
            }
            Debug.Log($"[Lock-On] Objetivo fijado: {currentTarget.name}");
        }
    }

    void UnlockTarget()
    {
        isLockedOn = false;
        currentTarget = null;

        if (cinemachineCam != null)
        {
            cinemachineCam.LookAt = transform; 
        }
        Debug.Log("[Lock-On] Objetivo liberado");
    }
}