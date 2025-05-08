using UnityEngine;

public class ArtilleryAIBehaviour : ArtilleryBaseBehaviour
{
    [Header("AI Configuration")]
    public SO_ArtilleryAIProfile aiProfile;
    public Transform target; // Assign the target transform in the Inspector or via script

    private float repositionTimer = 0f;

    protected override void Start()
    {
        base.Start();
        // Initialize AI-specific settings if needed
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
        // Implement logic to decide whether to fire based on aimingAccuracy
        return Random.value <= aiProfile.aimingAccuracy;
    }

    private void Reposition()
    {
        // Implement repositioning logic based on strategyType
        switch (aiProfile.strategyType)
        {
            case AI_StrategyType.Aggressive:
                // Move closer to the enemy
                break;
            case AI_StrategyType.Defensive:
                // Move to a safer location
                break;
            case AI_StrategyType.Balanced:
                // Maintain current position or make minor adjustments
                break;
            case AI_StrategyType.Random:
                // Move to a random position within certain bounds
                break;
        }
    }

    private void AimAtTarget(Transform target)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetVelocity = targetRb != null ? targetRb.linearVelocity : Vector3.zero;

        Vector3 aimPoint = PredictImpactPoint(target.position, targetVelocity, artilleryLauncher.muzzleVelocity, firePoint.position);

        Vector3 direction = aimPoint - firePoint.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        horizontalPivotPoint.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        verticalPivotPoint.localRotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0f, 0f);
    }

    private Vector3 PredictImpactPoint(Vector3 targetPos, Vector3 targetVelocity, float projectileSpeed, Vector3 shooterPos)
    {
        Vector3 displacement = targetPos - shooterPos;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;

        float targetSpeed = targetVelocity.magnitude;
        float cosTheta = Mathf.Cos(targetMoveAngle);

        float a = projectileSpeed * projectileSpeed - targetSpeed * targetSpeed;
        float b = 2f * displacement.magnitude * targetSpeed * cosTheta;
        float c = -displacement.sqrMagnitude;

        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0f)
        {
            // No solution, cannot hit the target with current projectile speed
            return targetPos;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDiscriminant) / (2f * a);
        float t2 = (-b - sqrtDiscriminant) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f)
            t = Mathf.Max(t1, t2);

        if (t > 0f)
        {
            return targetPos + targetVelocity * t;
        }
        else
        {
            // No valid time, return current position
            return targetPos;
        }
    }
}


