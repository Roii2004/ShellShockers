using UnityEngine;

[CreateAssetMenu(fileName = "SO_Projectiles", menuName = "Scriptable Objects/Projectiles")]
public class SO_Projectiles : ScriptableObject
{
    [Header("Basic Info")]
    public string projectileName;

    [Header("VFX")]
    public GameObject onImpactEffect;

    [Header("Explosion Detection")]
    public float detectionRadius = 1f;
    public float detectionDistance = 20f;
    public float earlyCameraTriggerSeconds = 2f;
    public LayerMask collisionMask;
}
