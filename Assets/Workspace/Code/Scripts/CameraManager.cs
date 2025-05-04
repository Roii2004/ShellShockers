using System;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineCamera projectileCam;
    public CinemachineCamera explosionCam;
    public float explosionCamDuration = 3f;  // duration of the isometric view
    
    private bool isFollowing;
    private GameObject toFollowProjectile;

    private void OnEnable()
    {
        ArtilleryBaseBehaviour.GetCurrentProjectile += GetProjectile;
        OrdnanceBaseBehaviour.OnOrdnanceTriggered += ExplosionCameraTrigger;
    }

    private void OnDisable()
    {
        ArtilleryBaseBehaviour.GetCurrentProjectile -= GetProjectile;
        OrdnanceBaseBehaviour.OnOrdnanceTriggered -= ExplosionCameraTrigger;
    }

    private void Start()
    {
        explosionCam.enabled = false;
        projectileCam.enabled = false;
    }

    void Update()
    {
        InputConsequences();
    }

    private void InputConsequences()
    {
        if (/*Input.GetKey(KeyCode.F) && */toFollowProjectile != null)
        {
            FollowProjectile(toFollowProjectile.transform);
        }
        else
        {
            StopFollowProjectile();
        }
    }

    void FollowProjectile(Transform projectile)
    {
        projectileCam.Follow = projectile.transform;
        projectileCam.LookAt = projectile.transform;
        
        projectileCam.enabled = true;
        isFollowing = true;
    }
    
    private void StopFollowProjectile()
    {
        projectileCam.enabled = false;
        isFollowing = false;
    }
    
    public void GetProjectile(GameObject projectile)
    {
        toFollowProjectile = projectile;
    }

    public void ExplosionCameraTrigger(Vector3 explosionPosition,Rigidbody rb)
    {
        // explosionCam.Priority = 30;
        // projectileCam.Priority = 10;
        //
        // explosionCam.transform.position = explosionPosition + new Vector3(0, 40, -40);
        // explosionCam.transform.LookAt(explosionPosition);
        //
        // explosionCam.enabled = true;
        // projectileCam.enabled = false;
        //
        //
        // StartCoroutine(DisableExplosionCamAfterDelay());
        
        explosionCam.Priority = 30;
        projectileCam.Priority = 10;

        // Dynamic isometric offset based on projectile's approach direction
        Vector3 incomingDirection = -rb.linearVelocity.normalized;  // needs access to Rigidbody
        Vector3 offset = incomingDirection * 30f + Vector3.up * 20f;

        explosionCam.transform.position = explosionPosition + offset;
        explosionCam.transform.LookAt(explosionPosition);

        explosionCam.enabled = true;
        projectileCam.enabled = false;

        StartCoroutine(DisableExplosionCamAfterDelay());
    }

    private IEnumerator DisableExplosionCamAfterDelay()
    {
        yield return new WaitForSeconds(explosionCamDuration);

        explosionCam.Priority = 0;
        explosionCam.enabled = false;

        projectileCam.Priority = 20;
        projectileCam.enabled = true;
    }
}
