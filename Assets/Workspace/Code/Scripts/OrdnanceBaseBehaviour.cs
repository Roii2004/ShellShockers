using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class OrdnanceBaseBehaviour : MonoBehaviour
{
    [Header("SO Settings")] 
    public SO_Projectiles projectileSettings;
    
    [Header("VFX Logic Settings (Place visual model GO)")] 
    public Transform visualModelTransform;  

    public static Action<Vector3,Rigidbody> OnOrdnanceTriggered;
    private Rigidbody rb;
    private bool hasTriggeredCamera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        DetectUpcomingCollision();
        VFXLogic();
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
            OnOrdnanceTriggered?.Invoke(impactPosition,rb);
        }
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

    private void VFXLogic()
    {
        visualModelTransform.Rotate(Vector3.forward, projectileSettings.spinSpeed * Time.deltaTime, Space.Self);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }
}
