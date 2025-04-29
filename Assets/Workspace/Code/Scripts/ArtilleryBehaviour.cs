using UnityEngine;
using UnityEngine.Serialization;

public class ArtilleryBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_ArtilleryLauncher artilleryLauncher;
    
    [Header("Pivot Points")]
    public Transform verticalPivotPoint;    // Tilts up/down (pitch)
    public Transform horizontalPivotPoint;  // Rotates left/right (yaw)

    [Header("Projectile Setup")]
    public GameObject shellPrefab;
    public Transform firePoint;
    
    private float timeSinceLastShot = 0f;
    private float fireCooldown;

    void Start()
    {
        fireCooldown = 60f / artilleryLauncher.roundsPerMinute;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        PivotRotation();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryFire();
        }
    }

    private void PivotRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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

    private void TryFire()
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
    
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
