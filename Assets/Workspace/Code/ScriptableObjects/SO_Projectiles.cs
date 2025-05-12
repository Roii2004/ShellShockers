using UnityEngine;

[CreateAssetMenu(fileName = "SO_Projectiles", menuName = "Scriptable Objects/Projectiles")]
public class SO_Projectiles : ScriptableObject
{
    [Header("Basic Info")]
    public string projectileName;
    public bool isPlayerProjectile=false;

    [Header("VFX")]
    public GameObject onImpactEffect;
    public float spinSpeed = 360f;
    public float tiltSpeed = 5f;
    public Vector3 modelRotationOffset = new Vector3(-90, 0, 0); // or whatever is needed

    [Header("Explosion Detection")]
    public float detectionRadius = 1f;
    public float earlyCameraTriggerSeconds = 2f;
    public LayerMask collisionMask;
}
