using UnityEngine;

[CreateAssetMenu(fileName = "SO_ArtilleryLauncher", menuName = "Scriptable Objects/ArtilleryLauncher")]
public class SO_ArtilleryLauncher : ScriptableObject
{
    [Header("Basic Info")]
    public string artilleryName;
    [Header("Firepower")]
    public float roundsPerMinute;
    public float muzzleVelocity;

    [Header("Setting")] 
    public float maxTilt;
    public float tiltSpeed;
    public Transform tiltPivot;
}
