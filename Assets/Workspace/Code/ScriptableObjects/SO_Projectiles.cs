using UnityEngine;

[CreateAssetMenu(fileName = "SO_Projectiles", menuName = "Scriptable Objects/Projectiles")]
public class SO_Projectiles : ScriptableObject
{
    [Header("Basic Info")]
    public string projectileName;
    
    [Header("VFX")]
    public GameObject onImpactEffect;
}
