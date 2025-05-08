using UnityEngine;

[CreateAssetMenu(fileName = "NewArtilleryAIProfile", menuName = "AI/Artillery AI Profile")]
public class SO_ArtilleryAIProfile : ScriptableObject
{
    [Range(0f, 1f)]
    public float aimingAccuracy = 0.75f; 

    public AI_StrategyType strategyType = AI_StrategyType.Aggressive;

    public float fireCooldownMultiplier = 1.0f; 

    public float detectionRadius = 50f; 

    public float repositionInterval = 10f; 

}

public enum AI_StrategyType
{
    Aggressive,
    Defensive,
    Balanced,
    Random
}