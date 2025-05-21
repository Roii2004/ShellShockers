using System;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
   [Header("Camera Settings")]
    public CinemachineCamera projectileCam;
    public CinemachineCamera explosionCam;
    public float explosionCamDuration = 3f;

    private bool isFollowing;
    private GameObject toFollowProjectile;

    private void OnEnable()
    {
        ArtilleryPlayableBehaviour.GetCurrentProjectile += GetProjectile;
        OrdnanceBaseBehaviour.onExplosionDetectionTriggered += ExplosionCameraTrigger;
    }

    private void OnDisable()
    {
        ArtilleryPlayableBehaviour.GetCurrentProjectile -= GetProjectile;
        OrdnanceBaseBehaviour.onExplosionDetectionTriggered -= ExplosionCameraTrigger;
    }

    private void Start()
    {
        explosionCam.enabled = false;
        projectileCam.enabled = false;
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && toFollowProjectile != null)
        {
            FollowProjectile(toFollowProjectile.transform);
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StopFollowProjectile();
        }
    }

    private void FollowProjectile(Transform projectile)
    {
        projectileCam.Follow = projectile;
        projectileCam.LookAt = projectile;

        projectileCam.Priority = 20;
        projectileCam.enabled = true;
        isFollowing = true;
    }

    private void StopFollowProjectile()
    {
        projectileCam.Priority = 0;
        projectileCam.enabled = false;
        isFollowing = false;
    }

    public void GetProjectile(GameObject projectile)
    {
        toFollowProjectile = projectile;
    }

    public void ExplosionCameraTrigger(Vector3 explosionPosition, Rigidbody rb)
    {
        if (toFollowProjectile != null && rb != null && rb.gameObject == toFollowProjectile)
        {
            explosionCam.Priority = 30;
            projectileCam.Priority = 10;

            Vector3 incomingDirection = -rb.linearVelocity.normalized;
            Vector3 offset = incomingDirection * 30f + Vector3.up * 20f;

            explosionCam.transform.position = explosionPosition + offset;
            explosionCam.transform.LookAt(explosionPosition);

            explosionCam.enabled = true;
            projectileCam.enabled = false;

            StartCoroutine(DisableExplosionCamAfterDelay());
        }
        else
        {
            Debug.Log("Explosion from a non-player projectile. Ignored.");
        }
    }

    private IEnumerator DisableExplosionCamAfterDelay()
    {
        yield return new WaitForSeconds(explosionCamDuration);

        explosionCam.Priority = 0;
        explosionCam.enabled = false;

        // Restore projectileCam only if F is still held down
        if (isFollowing && Input.GetKey(KeyCode.F))
        {
            projectileCam.Priority = 20;
            projectileCam.enabled = true;
        }
    }
}
