using UnityEngine;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
    [Header("AI Configuration")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target;

    private float repositionTimer = 0f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            AimAtTarget(target);

            if (ShouldFire())
            {
                TryFire();
            }

            repositionTimer += Time.deltaTime;
            if (repositionTimer >= aiProfile.repositionInterval)
            {
                Reposition();
                repositionTimer = 0f;
            }
        }
    }

    private bool ShouldFire()
    {
        return Random.value <= aiProfile.aimingAccuracy;
    }

    private void Reposition()
    {
        switch (aiProfile.strategyType)
        {
            case AI_StrategyType.Aggressive:
                break;
            case AI_StrategyType.Defensive:
                break;
            case AI_StrategyType.Balanced:
                break;
            case AI_StrategyType.Random:
                break;
        }
    }

    private void AimAtTarget(Transform target)
    {
        Vector3 targetPos = target.position;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetVelocity = targetRb != null ? targetRb.linearVelocity : Vector3.zero;

        Vector3 predictedPoint = PredictImpactPoint(targetPos, targetVelocity, artilleryLauncher.muzzleVelocity, firePoint.position);
        Vector3 toTarget = predictedPoint - firePoint.position;

        // Compute horizontal rotation
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float yaw = Quaternion.LookRotation(toTargetXZ).eulerAngles.y;
        horizontalPivotPoint.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Recalculate pitch relative to rotated horizontal pivot
        float distanceXZ = toTargetXZ.magnitude;
        float height = toTarget.y;
        float velocity = artilleryLauncher.muzzleVelocity;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float angle;
        if (CalculateLaunchAngle(velocity, distanceXZ, height, gravity, out angle))
        {
            float pitch = Mathf.Rad2Deg * angle;

            // Clamp elevation angle
            pitch = Mathf.Clamp(pitch, 15f, 75f);

            // Apply pitch to vertical pivot in local space
            verticalPivotPoint.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
        else
        {
            verticalPivotPoint.localRotation = Quaternion.Euler(45f, 0f, 0f); // fallback
        }
    }

    private bool CalculateLaunchAngle(float velocity, float distanceXZ, float deltaY, float gravity, out float angle)
    {
        float v2 = velocity * velocity;
        float v4 = v2 * v2;
        float root = v4 - gravity * (gravity * distanceXZ * distanceXZ + 2f * deltaY * v2);

        if (root < 0f)
        {
            angle = 0f;
            return false;
        }

        float sqrt = Mathf.Sqrt(root);
        float highAngle = Mathf.Atan((v2 + sqrt) / (gravity * distanceXZ));
        angle = highAngle;
        return true;
    }

    private Vector3 PredictImpactPoint(Vector3 targetPos, Vector3 targetVelocity, float projectileSpeed, Vector3 shooterPos)
    {
        Vector3 toTarget = targetPos - shooterPos;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float distance = toTargetXZ.magnitude;

        float angle;
        if (!CalculateLaunchAngle(projectileSpeed, distance, toTarget.y, Mathf.Abs(Physics.gravity.y), out angle))
            return targetPos;

        float time = distance / (projectileSpeed * Mathf.Cos(angle));
        Vector3 predictedPos = targetPos + targetVelocity * time;

        return predictedPos;
    }
}

