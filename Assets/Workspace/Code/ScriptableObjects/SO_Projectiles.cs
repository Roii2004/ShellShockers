using UnityEngine;

[CreateAssetMenu(fileName = "SO_Projectiles", menuName = "Scriptable Objects/Projectiles")]
public class SO_Projectiles : ScriptableObject
{
    [Header("Basic Info")]
    public string projectileName;

    [Header("VFX")]
    public GameObject onImpactEffect;
    public float spinSpeed = 720f; 


    [Header("Explosion Detection")]
    public float detectionRadius = 1f;
    public float earlyCameraTriggerSeconds = 2f;
    public LayerMask collisionMask;
}
