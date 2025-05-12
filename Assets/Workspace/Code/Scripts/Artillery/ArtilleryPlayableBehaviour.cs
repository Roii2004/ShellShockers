using UnityEngine;
using System;
using System.Collections.Generic;

public class ArtilleryPlayableBehaviour : ArtilleryBaseBehaviour
{
    [Header("Trajectory Preview")]
    public LineRenderer trajectoryLine;
    public int resolution = 30;  // Number of points in the arc
    public float maxSimTime = 5f;  // Max time to simulate
    
    public static Action<GameObject> GetCurrentProjectile;

    protected override void Start()
    {
        base.Start();
        if (trajectoryLine != null)
            trajectoryLine.enabled = true;
    }

    protected override void Update()
    {
        base.Update();

        PlayerInput();
    }

    private void PlayerInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        PivotRotation(horizontalInput, verticalInput);

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
            if (trajectoryLine != null)
                trajectoryLine.positionCount = 0;
        }
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

                    GetCurrentProjectile?.Invoke(shell);

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
    
    protected void DrawTrajectory()
    {
        if (!trajectoryLine || !firePoint) return;

        List<Vector3> points = new List<Vector3>();

        Vector3 startPos = firePoint.position;
        Vector3 startVelocity = firePoint.forward * artilleryLauncher.muzzleVelocity;

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

