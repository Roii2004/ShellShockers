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

    [PunRPC]
    public void TakeDamage(float amount)
    {
        _health -= amount;
        Debug.Log( "FROM EDITOR"+gameObject.name + " has" + _health);
    }
}
