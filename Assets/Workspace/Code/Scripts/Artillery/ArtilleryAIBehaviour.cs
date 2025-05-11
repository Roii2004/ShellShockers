using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
   [Header("AI Settings")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target;

    private const float alignmentTolerance =1f; // degrees

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

        // Calculate the desired launch direction using yaw + pitch
        Quaternion yawRot = Quaternion.Euler(0f, desiredYaw, 0f);
        Quaternion pitchRot = Quaternion.Euler(-desiredPitch, 0f, 0f); // -pitch: Unity pitches down for +X
        Vector3 desiredLaunchDir = yawRot * pitchRot * Vector3.forward;

        // Compare with current direction
        Vector3 currentDir = firePoint.forward;
        float angleDiff = Vector3.Angle(currentDir, desiredLaunchDir);
        Vector3 cross = Vector3.Cross(currentDir, desiredLaunchDir);

        // Use constant input based on sign (no slowdown near target)
        float horizontalInput = Mathf.Sign(cross.y);   // Y axis = yaw
        float verticalInput = Mathf.Sign(-cross.x);    // X axis = pitch (negative due to Unity's convention)

        PivotRotation(horizontalInput, verticalInput);

        // Aim is "close enough" when angle difference is small
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

        // Horizontal direction
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
            return false; // No valid firing arc
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float angleHigh = Mathf.Atan((v2 + sqrtDiscriminant) / (gravity * x));
        requiredPitchHigh = angleHigh * Mathf.Rad2Deg;

        // Convert world-space direction to mortar's local yaw
        Vector3 flatDirection = toTargetXZ.normalized;
        requiredYaw = Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg;

        return true;
    }
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * 5f); // Shows current fire direction

            if (target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(firePoint.position, target.position); // Target line

                // Draw desired ballistic arc (optional)
                Vector3 firePos = firePoint.position;
                Vector3 targetPos = target.position;
                float v = artilleryLauncher != null ? artilleryLauncher.muzzleVelocity : 10f;
                float g = Mathf.Abs(Physics.gravity.y);

                if (CalculateMortarAngles(firePos, targetPos, v, g, out float yaw, out float pitch))
                {
                    // Simulate a parabolic arc using physics steps
                    Vector3 dir = (targetPos - firePos).normalized;
                    Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
                    Quaternion pitchRot = Quaternion.Euler(-pitch, 0f, 0f); // NEGATIVE because Unity pitches down by +X

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


