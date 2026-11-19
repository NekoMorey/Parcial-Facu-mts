using UnityEngine;
using Unity.Cinemachine;

public class FijarObjetivo : MonoBehaviour
{
    [Header("Configuracion de Lock-On")]
    public KeyCode lockKey = KeyCode.R;
    public float targetNoticeDistance = 25f;
    public LayerMask targetLayers;

    [Header("Referencias")]
    public CinemachineCamera cinemachineCam;
    public Transform currentTarget;

    [Header("Camara")]
    public float multiplicadorSensibilidad = 4f;

    public bool IsLockedOn { get; private set; } = false;

    private CinemachineOrbitalFollow orbitalFollow;

    void Start()
    {
        if (cinemachineCam != null)
        {
            orbitalFollow = cinemachineCam.GetComponent<CinemachineOrbitalFollow>();
            SubirSensibilidad();
        }
    }

    void SubirSensibilidad()
    {
        var ejes = cinemachineCam.GetComponent<CinemachineInputAxisController>();
        if (ejes == null) return;
        foreach (var control in ejes.Controllers)
            control.Input.Gain *= multiplicadorSensibilidad;
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

        if (IsLockedOn && currentTarget == null)
        {
            UnlockTarget();
        }
        else if (IsLockedOn)
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
            orbitalFollow.HorizontalAxis.Value = Mathf.LerpAngle(orbitalFollow.HorizontalAxis.Value, targetAngle, Time.deltaTime * 8f);
        }

        Vector3 fullDir = currentTarget.position - transform.position;
        float distanceXZ = Mathf.Sqrt(fullDir.x * fullDir.x + fullDir.z * fullDir.z);
        if (distanceXZ > 0.01f)
        {
            float pitch = Mathf.Clamp(-Mathf.Atan2(fullDir.y, distanceXZ) * Mathf.Rad2Deg + 14f, orbitalFollow.VerticalAxis.Range.x, orbitalFollow.VerticalAxis.Range.y);
            orbitalFollow.VerticalAxis.Value = Mathf.LerpAngle(orbitalFollow.VerticalAxis.Value, pitch, Time.deltaTime * 5f);
        }
    }

    void LockOnClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetNoticeDistance, targetLayers);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider hit in hits)
        {
            IDanable enemy = hit.GetComponentInParent<IDanable>();
            if (enemy == null) continue;

            Transform enemyTransform = ((Component)enemy).transform;
            float distance = Vector3.Distance(transform.position, enemyTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = enemyTransform;
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