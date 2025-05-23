using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrdnanceBaseBehaviour : MonoBehaviour
{
    [Header("SO Settings")] 
    public SO_Projectiles projectileSettings;
    
    [Header("VFX Logic Settings (Place visual model GO)")] 
    public Transform visualModelTransform;  

    public static Action<Vector3,Rigidbody> onExplosionDetectionTriggered;
    private Rigidbody rb;
    private bool hasTriggeredCamera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
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

            onExplosionDetectionTriggered?.Invoke(impactPosition,rb);
        }
    }
    
    private void Explode()
    {
        if (projectileSettings != null && projectileSettings.onImpactEffect != null)
        {
            Instantiate(projectileSettings.onImpactEffect,
                transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void VFXLogic()
    {
        visualModelTransform.Rotate(Vector3.up, projectileSettings.spinSpeed * Time.deltaTime, Space.Self);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 velocityDirection = rb.linearVelocity.normalized;

            Vector3 rotationAxis = Vector3.Cross(Vector3.up, velocityDirection);  
            float angle = Vector3.Angle(Vector3.up, velocityDirection);  

            Quaternion targetRotation = Quaternion.AngleAxis(angle, rotationAxis);
            visualModelTransform.rotation = Quaternion.Slerp(visualModelTransform.rotation, targetRotation, Time.deltaTime * projectileSettings.tiltSpeed);

            Quaternion modelCorrection = Quaternion.Euler(projectileSettings.modelRotationOffset);
            visualModelTransform.rotation = Quaternion.LookRotation(velocityDirection) * modelCorrection;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    
    
}
