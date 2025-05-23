using UnityEngine;

[CreateAssetMenu(fileName = "SO_ArtilleryLauncher", menuName = "Scriptable Objects/ArtilleryLauncher")]
public class SO_ArtilleryLauncher : ScriptableObject
{
    [Header("Basic Info")]
    public string artilleryName;
    
    [Header("Firepower")]
    public float roundsPerMinute;
    public float muzzleVelocity;
    public float recoilAmount = 2f;
    
    [Header("Setting")] 
    public float tiltSpeed;
    
    [Header("Vertical Tilt Limits")]
    [Range(-90f, 90f)] public float minVerticalTilt = 0f;
    [Range(-90f, 90f)] public float maxVerticalTilt = 60f;

    [Header("Horizontal Tilt Limits")]
    [Range(-180f, 180f)] public float minHorizontalTilt = -30f;
    [Range(-180f, 180f)] public float maxHorizontalTilt = 30f;
    
    [Header("VFX")]
    public GameObject muzzleEffects;
}
