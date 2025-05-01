using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class OrdnanceBehaviour : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SO_Projectiles ordnanceData;

    [Header("Other")] 
    public float verticalOffset=1;

    private void OnTriggerEnter(Collider other)
    {   
        Destroy(gameObject);
        GameObject aux = Instantiate(ordnanceData.onImpactEffect,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + verticalOffset,
                gameObject.transform.position.z), Quaternion.identity);
    }
}
