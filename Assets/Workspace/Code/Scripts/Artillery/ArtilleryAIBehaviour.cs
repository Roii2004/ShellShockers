using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
    [Header("AI Settings")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target;

    private void Update()
    {
        base.Update();

        if (target == null) return;

        AimAtTarget();
    }

    private void AimAtTarget()
    {
        Vector3 directionToTarget = target.position - firePoint.position;

        // Horizontal (Yaw)
        Vector3 flatDirection = new Vector3(directionToTarget.x, 0f, directionToTarget.z);
        Quaternion desiredYawRotation = Quaternion.LookRotation(flatDirection);
        float desiredYaw = NormalizeAngle(desiredYawRotation.eulerAngles.y);
        float currentYaw = NormalizeAngle(horizontalPivotPoint.localEulerAngles.y);
        float yawDelta = desiredYaw - currentYaw;
        float horizontalInput = Mathf.Clamp(yawDelta / 30f, -1f, 1f); // scale to input

        // Vertical (Pitch)
        Quaternion fullLookRotation = Quaternion.LookRotation(directionToTarget);
        float desiredPitch = NormalizeAngle(fullLookRotation.eulerAngles.x);
        float currentPitch = NormalizeAngle(verticalPivotPoint.localEulerAngles.x);
        float pitchDelta = desiredPitch - currentPitch;
        float verticalInput = Mathf.Clamp(pitchDelta / 30f, -1f, 1f); // scale to input

        PivotRotation(horizontalInput, verticalInput);
    }
}

