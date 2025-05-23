using UnityEngine;
using System.Collections;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
    [Header("AI Settings")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target;

    private const float alignmentTolerance = 3f; // degrees
    private float currentAccuracy;

    private Vector3 currentInaccurateTarget;
    private bool hasTargetLock = false;

    private void Start()
    {
        base.Start();
        currentAccuracy = aiProfile.aimingAccuracy;
    }

    private void Update()
    {
        base.Update();

        if (target == null) return;

        if (!hasTargetLock)
        {
            currentInaccurateTarget = GetInaccurateTarget(target.position);
            hasTargetLock = true;
        }
        else
        {
            //code here
        }

        if (AimWithBallistics(out bool isAimed) && isAimed)
        {
            TryFire();
        }

        print(currentAccuracy);
    }

    private bool AimWithBallistics(out bool isAimed)
    {
        isAimed = false;

        Vector3 firePosition = firePoint.position;
        Vector3 targetPosition = currentInaccurateTarget; // Stable target

        float muzzleVelocity = artilleryLauncher.muzzleVelocity;
        float gravity = Mathf.Abs(Physics.gravity.y);

        if (!CalculateMortarAngles(firePosition, targetPosition, muzzleVelocity, gravity, out float desiredYaw, out float desiredPitch))
            return false;

        Quaternion yawRot = Quaternion.Euler(0f, desiredYaw, 0f);
        Quaternion pitchRot = Quaternion.Euler(-desiredPitch, 0f, 0f);
        Vector3 desiredLaunchDir = yawRot * pitchRot * Vector3.forward;

        Vector3 currentDir = firePoint.forward;
        float angleDiff = Vector3.Angle(currentDir, desiredLaunchDir);
        Vector3 cross = Vector3.Cross(currentDir, desiredLaunchDir);

        float horizontalInput = Mathf.Sign(cross.y);
        float verticalInput = Mathf.Sign(-cross.x);

        PivotRotation(horizontalInput, verticalInput);

        isAimed = angleDiff <= alignmentTolerance;
        return true;
    }

    private Vector3 GetInaccurateTarget(Vector3 actualTargetPos)
    {
        if (currentAccuracy >= 1f) return actualTargetPos;

        float maxOffset = 20f; // Stronger inaccuracy
        float inaccuracy = 1f - currentAccuracy;
        float radius = maxOffset * Mathf.Pow(inaccuracy, 2f); // Non-linear falloff

        Vector2 offset2D = Random.insideUnitCircle * radius;
        Vector3 offset = new Vector3(offset2D.x, 0f, offset2D.y);

        return actualTargetPos + offset;
    }

    private bool CalculateMortarAngles(
        Vector3 firePosition,
        Vector3 targetPosition,
        float muzzleVelocity,
        float gravity,
        out float requiredYaw,
        out float requiredPitchHigh)
    {
        Vector3 toTarget = targetPosition - firePosition;

        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float x = toTargetXZ.magnitude;
        float y = toTarget.y;

        float v = muzzleVelocity;
        float v2 = v * v;
        float discriminant = v2 * v2 - gravity * (gravity * x * x + 2 * y * v2);

        if (discriminant < 0f)
        {
            requiredYaw = 0f;
            requiredPitchHigh = 0f;
            return false;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float angleHigh = Mathf.Atan((v2 + sqrtDiscriminant) / (gravity * x));
        requiredPitchHigh = angleHigh * Mathf.Rad2Deg;

        Vector3 flatDirection = toTargetXZ.normalized;
        requiredYaw = Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg;

        return true;
    }

    protected override void TryFire()
    {
        if (timeSinceLastShot >= fireCooldown)
        {
            timeSinceLastShot = 0f;

            if (shellPrefab && firePoint)
            {
                GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
                Rigidbody rb = shell.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = firePoint.forward * artilleryLauncher.muzzleVelocity;
                    //Fire is different in base, playable and AI mortar
                    Fire(rb);
                }
                else
                {
                    Debug.LogWarning("Shell prefab has no Rigidbody.");
                }
            }
            else
            {
                Debug.LogWarning("ShellPrefab or FirePoint not assigned.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null || target == null || artilleryLauncher == null || aiProfile == null)
            return;

        Vector3 firePos = firePoint.position;
        Vector3 actualTarget = target.position;
        Vector3 inaccurateTarget = currentInaccurateTarget;

        // Line to real target
        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePos, actualTarget);
        Gizmos.DrawSphere(actualTarget, 0.5f);

        // Line to inaccurate target
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePos, inaccurateTarget);
        Gizmos.DrawSphere(inaccurateTarget, 0.5f);

        // Draw the inaccuracy radius at target
        float maxOffset = 20f;
        float radius = maxOffset * Mathf.Pow(1f - currentAccuracy, 2f);

        Gizmos.color = Color.red;
        int segments = 32;
        Vector3 center = actualTarget;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;
           //hola perro
            Gizmos.DrawLine(point1, point2);
        }
    }

    public override void Fire(Rigidbody rb)
    {
        base.Fire(rb);
        ApplyRecoil();

        // Improve accuracy after shot
        currentAccuracy = Mathf.Min(1f, currentAccuracy + 0.1f);
        hasTargetLock = false;
    }
}