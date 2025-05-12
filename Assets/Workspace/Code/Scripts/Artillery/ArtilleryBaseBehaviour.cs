using UnityEngine;
using System.Collections;

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
                    rb.linearVelocity = firePoint.forward * artilleryLauncher.muzzleVelocity;

                    //ApplyRecoil();

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
        StopCoroutine(nameof(BlendRecoil)); // Stop if already blending
        StartCoroutine(BlendRecoil());
    }

    private IEnumerator BlendRecoil()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        // Store current local rotations
        Quaternion startVert = verticalPivotPoint.localRotation;
        Quaternion startHoriz = horizontalPivotPoint.localRotation;

        // Get current angles
        float currentX = NormalizeAngle(verticalPivotPoint.localEulerAngles.x);
        float currentY = NormalizeAngle(horizontalPivotPoint.localEulerAngles.y);

        // Apply backward vertical recoil only (always tilts up)
        float recoilX = currentX - Random.Range(0f, artilleryLauncher.recoilAmount);
        recoilX = Mathf.Clamp(recoilX, artilleryLauncher.minVerticalTilt, artilleryLauncher.maxVerticalTilt);
        Quaternion targetVert = Quaternion.Euler(recoilX, 0f, 0f);

        // Optional: slight horizontal wiggle (left or right)
        float recoilY = currentY + Random.Range(-artilleryLauncher.recoilAmount, artilleryLauncher.recoilAmount);
        recoilY = Mathf.Clamp(recoilY, artilleryLauncher.minHorizontalTilt, artilleryLauncher.maxHorizontalTilt);
        Quaternion targetHoriz = Quaternion.Euler(0f, recoilY, 0f);

        // Smooth blend
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            verticalPivotPoint.localRotation = Quaternion.Slerp(startVert, targetVert, t);
            horizontalPivotPoint.localRotation = Quaternion.Slerp(startHoriz, targetHoriz, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact target rotations
        verticalPivotPoint.localRotation = targetVert;
        horizontalPivotPoint.localRotation = targetHoriz;
    }
    protected float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
