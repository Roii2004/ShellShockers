using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Pun;
[RequireComponent(typeof(PhotonView))]
public class ArtilleryPlayableBehaviour : ArtilleryBaseBehaviour, IPunObservable
{
    [Header("Trajectory Preview")]
    public LineRenderer trajectoryLine;
    public int resolution = 30;  // Number of points in the arc
    public float maxSimTime = 5f;  // Max time to simulate
    
    public static Action<GameObject> GetCurrentProjectile;
    private PhotonView _photonView;
    protected override void Start()
    {
        base.Start();
        
        _photonView = GetComponent<PhotonView>();
        
        if (trajectoryLine != null)
            trajectoryLine.enabled = true;
    }

    protected override void Update()
    {
        base.Update();

        
        if (_photonView.IsMine)
        {
            PlayerInput();
        }
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
                GameObject shell =PhotonNetwork.Instantiate("NetworkPrefabs/Round", firePoint.position, firePoint.rotation);
                Rigidbody rb = shell.GetComponent<Rigidbody>();

                PhotonView shellView = shell.GetComponent<PhotonView>();
                if (shellView == null)
                {
                    Debug.LogError("Shell instantiated without a PhotonView! Ensure that both the root GameObject and the visual child of the shell prefab have a PhotonView and PhotonTransformView.");
                }

                if (rb != null)
                {
                    Fire(rb);
                    
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
    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Only send vertical pivot, horizontal is handled by PhotonTransformView
            stream.SendNext(verticalPivotPoint.localEulerAngles);
        }
        else
        {
            Vector3 verticalRotation = (Vector3)stream.ReceiveNext();
            verticalPivotPoint.localEulerAngles = verticalRotation;
        }
    }

    public override void Fire(Rigidbody rb)
    {
        //Fire is different in base, playable and AI mortar
        photonView.RPC("RPC_ShowMuzzleFlash", RpcTarget.All, firePoint.position);
        base.Fire(rb);
        ApplyRecoil();
    }
    
    [PunRPC]
    void RPC_ShowMuzzleFlash(Vector3 firingPointPosition)
    {
        Instantiate(artilleryLauncher.muzzleEffects, firingPointPosition , Quaternion.identity);
    }
}
