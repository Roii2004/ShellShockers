using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class OrdnanceBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_Projectiles ordnanceData;

    private void OnTriggerEnter(Collider other)
    {   
        Destroy(gameObject);
        GameObject aux = Instantiate(ordnanceData.onImpactEffect,gameObject.transform.position,Quaternion.identity);
        
    }
}
