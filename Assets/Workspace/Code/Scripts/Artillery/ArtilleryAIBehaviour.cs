using UnityEngine;
using System.Collections;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
    [Header("AI Settings")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target;

    private const float alignmentTolerance = 1f; // degrees

    private void Update()
    {
        base.Update();

        if (target == null) return;

        if (AimWithBallistics(out bool isAimed) && isAimed)
        {
            TryFire();
        }
    }

    private bool AimWithBallistics(out bool isAimed)
    {
        isAimed = false;

        Vector3 firePosition = firePoint.position;
        Vector3 targetPosition = target.position;
        float muzzleVelocity = artilleryLauncher.muzzleVelocity;
        float gravity = Mathf.Abs(Physics.gravity.y);

        if (!CalculateMortarAngles(firePosition, targetPosition, muzzleVelocity, gravity, out float desiredYaw, out float desiredPitch))
            return false; // No valid trajectory

        Quaternion yawRot = Quaternion.Euler(0f, desiredYaw, 0f);
        Quaternion pitchRot = Quaternion.Euler(-desiredPitch, 0f, 0f); // -pitch: Unity pitches down for +X
        Vector3 desiredLaunchDir = yawRot * pitchRot * Vector3.forward;

        Vector3 currentDir = firePoint.forward;
        float angleDiff = Vector3.Angle(currentDir, desiredLaunchDir);
        Vector3 cross = Vector3.Cross(currentDir, desiredLaunchDir);

        float horizontalInput = Mathf.Sign(cross.y);   // Y axis = yaw
        float verticalInput = Mathf.Sign(-cross.x);    // X axis = pitch

        PivotRotation(horizontalInput, verticalInput);

        isAimed = angleDiff <= alignmentTolerance;
        return true;
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

                    ApplyRecoil(); // RECOIL added after shot

                    Debug.Log("AI Shell launched with recoil!");
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
        else
        {
            Debug.Log("Still reloading...");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * 5f);

            if (target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(firePoint.position, target.position);

                Vector3 firePos = firePoint.position;
                Vector3 targetPos = target.position;
                float v = artilleryLauncher != null ? artilleryLauncher.muzzleVelocity : 10f;
                float g = Mathf.Abs(Physics.gravity.y);

                if (CalculateMortarAngles(firePos, targetPos, v, g, out float yaw, out float pitch))
                {
                    Vector3 dir = (targetPos - firePos).normalized;
                    Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
                    Quaternion pitchRot = Quaternion.Euler(-pitch, 0f, 0f);

                    Vector3 launchDir = yawRot * pitchRot * Vector3.forward;
                    Vector3 velocity = launchDir * v;

                    Gizmos.color = Color.cyan;
                    Vector3 pos = firePos;
                    for (float t = 0; t < 30f; t += 0.05f)
                    {
                        Vector3 next = pos + velocity * 0.1f + 0.5f * Physics.gravity * 0.1f * 0.1f;
                        Gizmos.DrawLine(pos, next);
                        velocity += Physics.gravity * 0.1f;
                        pos = next;
                    }
                }
            }
        }
    }
}


