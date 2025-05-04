using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class OrdnanceBaseBehaviour : MonoBehaviour
{
    [Header("SO Settings")] 
    public SO_Projectiles projectileSettings;

    public static Action<Vector3> OnOrdnanceTriggered;

    private Rigidbody rb;
    private bool hasTriggeredCamera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        DetectUpcomingCollision();
    }

    private void DetectUpcomingCollision()
    {
        if (hasTriggeredCamera || projectileSettings == null) return;

        Vector3 direction = rb.linearVelocity.normalized;
        float speed = rb.linearVelocity.magnitude;
        float futureDistance = speed * projectileSettings.earlyCameraTriggerSeconds;

        if (Physics.SphereCast(transform.position, projectileSettings.detectionRadius, direction,
                out RaycastHit hit, futureDistance, projectileSettings.collisionMask))
        {
            hasTriggeredCamera = true;
            Vector3 impactPosition = hit.point;

            Debug.Log("Upcoming collision detected. Triggering explosion camera early at " + impactPosition);
            OnOrdnanceTriggered?.Invoke(impactPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void Explode()
    {
        if (projectileSettings != null && projectileSettings.onImpactEffect != null)
        {
            Instantiate(projectileSettings.onImpactEffect,
                transform.position, Quaternion.identity);
        }

        Debug.Log("Shell exploded");
        Destroy(gameObject);
    }
}
