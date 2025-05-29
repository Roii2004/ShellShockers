using UnityEngine;
using Photon.Pun;

public class HealthBehaviour : MonoBehaviourPun, IDamageable
{
    public SO_AliveEntity aliveEntity;
    private float _health;
    void Start()
    {
        aliveEntity.health = _health;
    }
    

    [PunRPC]
    public void TakeDamage(float amount)
    {
        print( gameObject.name+ " has taken damage");
        _health -= amount;
        print( gameObject.name+ " has" + _health);

        if (_health <= 0)
        {
            //Die
        }
    }
}
