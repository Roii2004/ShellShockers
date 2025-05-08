using UnityEngine;
using System.Collections.Generic;

public abstract class ArtilleryBaseBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_ArtilleryLauncher artilleryLauncher;

    [Header("Pivot Points")]
    public Transform verticalPivotPoint;    // Tilts up/down (pitch)
    public Transform horizontalPivotPoint;  // Rotates left/right (yaw)

    [Header("Projectile Setup")]
    public GameObject shellPrefab;
    public Transform firePoint;
    

    protected float timeSinceLastShot = 0f;
    protected float fireCooldown;


    protected virtual void Start()
    {
        fireCooldown = 60f / artilleryLauncher.roundsPerMinute;
    }

    protected virtual void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    protected void PivotRotation(float horizontalInput, float verticalInput)
    {
        // Vertical tilt (elevation)
        float currentX = verticalPivotPoint.localEulerAngles.x;
        currentX = NormalizeAngle(currentX);
        float newX = currentX + verticalInput * artilleryLauncher.tiltSpeed * Time.deltaTime;
        newX = Mathf.Clamp(newX, artilleryLauncher.minVerticalTilt, artilleryLauncher.maxVerticalTilt);
        verticalPivotPoint.localRotation = Quaternion.Euler(newX, 0f, 0f);

        // Horizontal pivot (yaw)
        float currentY = horizontalPivotPoint.localEulerAngles.y;
        currentY = NormalizeAngle(currentY);
        float newY = currentY + horizontalInput * artilleryLauncher.tiltSpeed * Time.deltaTime;
        newY = Mathf.Clamp(newY, artilleryLauncher.minHorizontalTilt, artilleryLauncher.maxHorizontalTilt);
        horizontalPivotPoint.localRotation = Quaternion.Euler(0f, newY, 0f);
    }

    protected virtual void TryFire()
    {
        if (timeSinceLastShot >= fireCooldown)
        {
            timeSinceLastShot = 0f;

            if (shellPrefab && firePoint)
            {
                GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
                Rigidbody rb = shell.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = firePoint.up * artilleryLauncher.muzzleVelocity;

                    //GetCurrentProjectile?.Invoke(shell);

                    ApplyRecoil();

                    Debug.Log("Shell launched!");
                }
                else
                {
                    Debug.LogWarning("Shell prefab has no Rigidbody.");
                }
            }
            else
            {
                Debug.LogWarning("ShellPrefab or FirePoint not assigned.");
            }
        }
        else
        {
            Debug.Log("Still reloading...");
        }
    }

    protected void ApplyRecoil()
    {
        // Vertical tilt recoil
        float currentX = NormalizeAngle(verticalPivotPoint.localEulerAngles.x);
        float recoilX = currentX + UnityEngine.Random.Range(-artilleryLauncher.recoilAmount, artilleryLauncher.recoilAmount);
        recoilX = Mathf.Clamp(recoilX, artilleryLauncher.minVerticalTilt, artilleryLauncher.maxVerticalTilt);
        verticalPivotPoint.localRotation = Quaternion.Euler(recoilX, 0f, 0f);

        // Horizontal tilt recoil
        float currentY = NormalizeAngle(horizontalPivotPoint.localEulerAngles.y);
        float recoilY = currentY + UnityEngine.Random.Range(-artilleryLauncher.recoilAmount, artilleryLauncher.recoilAmount);
        recoilY = Mathf.Clamp(recoilY, artilleryLauncher.minHorizontalTilt, artilleryLauncher.maxHorizontalTilt);
        horizontalPivotPoint.localRotation = Quaternion.Euler(0f, recoilY, 0f);
    }

    protected float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
