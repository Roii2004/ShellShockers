using UnityEngine;

public class ArtilleryBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_ArtilleryLauncher artilleryLauncher;
    public float rotationSpeed = 10f;  
    public Transform rotationPivotPoint;

    void Update()
    {
        PivotRotation();
    }

    private void PivotRotation()
    {
        //Rotates the pivot with the player input
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rotationPivotPoint.Rotate(Vector3.right * vertical * rotationSpeed * Time.deltaTime); //Up Down
        rotationPivotPoint.Rotate(Vector3.up * horizontal * rotationSpeed * Time.deltaTime); //Right Left
    }
}
