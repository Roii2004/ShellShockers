using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class ArtilleryBaseBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_ArtilleryLauncher artilleryLauncher;
    
    [Header("Pivot Points")]
    public Transform verticalPivotPoint;    // Tilts up/down (pitch)
    public Transform horizontalPivotPoint;  // Rotates left/right (yaw)

    [Header("Projectile Setup")]
    public GameObject shellPrefab;
    public Transform firePoint;
    
    [Header("Trajectory Preview")]
    public LineRenderer trajectoryLine;
    public int resolution = 30;  // Number of points in the arc
    public float maxSimTime = 5f;  // Max time to simulate
    
    
    private float timeSinceLastShot = 0f;
    private float fireCooldown;
    
    public static Action<GameObject> GetCurrentProjectile;

    void Start()
    {
        fireCooldown = 60f / artilleryLauncher.roundsPerMinute;
        trajectoryLine.enabled = true;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        PivotRotation();

        InputActions();
    }

    private void InputActions()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryFire();
        }

        if (Input.GetKey(KeyCode.E))
        {
            DrawTrajectory();
        }
        else
        {
            // Clear the arc when E is not held
            trajectoryLine.positionCount = 0;
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
                    GetCurrentProjectile?.Invoke(shell);
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
    
    private void DrawTrajectory()
    {
        if (!trajectoryLine || !firePoint) return;

        List<Vector3> points = new List<Vector3>();
    
        Vector3 startPos = firePoint.position;
        Vector3 startVelocity = firePoint.up * artilleryLauncher.muzzleVelocity;

        Vector3 previousPoint = startPos;
        points.Add(previousPoint);

        for (int i = 1; i < resolution; i++)
        {
            float t = (i / (float)(resolution - 1)) * maxSimTime;
            Vector3 currentPoint = startPos + startVelocity * t + 0.5f * Physics.gravity * t * t;

            // Raycast from previous point to current point
            if (Physics.Raycast(previousPoint, currentPoint - previousPoint, out RaycastHit hit, Vector3.Distance(previousPoint, currentPoint)))
            {
                points.Add(hit.point);  // Stop at collision point
                break;
            }

            points.Add(currentPoint);
            previousPoint = currentPoint;
        }

        trajectoryLine.positionCount = points.Count;
        trajectoryLine.SetPositions(points.ToArray());
    }
}
