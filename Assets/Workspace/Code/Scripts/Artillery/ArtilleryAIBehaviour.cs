using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        // Normalize current angles
        float currentYaw = NormalizeAngle(horizontalPivotPoint.localEulerAngles.y);
        float currentPitch = NormalizeAngle(verticalPivotPoint.localEulerAngles.x);

        // Calculate angle differences
        float yawDelta = NormalizeAngle(desiredYaw - currentYaw);
        float pitchDelta = NormalizeAngle(desiredPitch - currentPitch);

        // Scaled rotation input
        float horizontalInput = Mathf.Clamp(yawDelta / 30f, -1f, 1f);
        float verticalInput = Mathf.Clamp(pitchDelta / 30f, -1f, 1f);

        PivotRotation(horizontalInput, verticalInput);

        // Check if aligned close enough
        isAimed = Mathf.Abs(yawDelta) <= alignmentTolerance && Mathf.Abs(pitchDelta) <= alignmentTolerance;
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

        // Horizontal distance
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float x = toTargetXZ.magnitude;

        // Vertical offset
        float y = toTarget.y;

        float v = muzzleVelocity;
        float v2 = v * v;
        float discriminant = v2 * v2 - gravity * (gravity * x * x + 2 * y * v2);

        if (discriminant < 0f)
        {
            requiredYaw = 0f;
            requiredPitchHigh = 0f;
            return false; // No solution
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);

        float angleLow = Mathf.Atan((v2 - sqrtDiscriminant) / (gravity * x));
        float angleHigh = Mathf.Atan((v2 + sqrtDiscriminant) / (gravity * x));
        requiredPitchHigh = angleHigh * Mathf.Rad2Deg; // Force mortar-style high arc

        // Horizontal yaw angle
        Vector3 flatDir = toTargetXZ.normalized;
        requiredYaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;

        return true;
    }

}


