using System;
using UnityEngine;
using Photon.Pun;

public class HealthBehaviour : MonoBehaviourPun, IDamageable
{
    public SO_AliveEntity aliveEntity;
    private float _health;
    void Start()
    {
        _health=aliveEntity.health;
    }

    public void TakeDamage(float amount)
    {
        
    }
}
