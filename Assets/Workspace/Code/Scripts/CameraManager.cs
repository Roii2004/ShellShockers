using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineCamera projectileCam;
    
    private bool isFollowing = false;
    private GameObject toFollowProjectile;

    private void OnEnable()
    {
        ArtilleryBaseBehaviour.GetCurrentProjectile += GetProjectile;
    }

    private void OnDisable()
    {
        ArtilleryBaseBehaviour.GetCurrentProjectile -= GetProjectile;
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
}
