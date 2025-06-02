using UnityEngine;
using Photon.Pun;
public class NetworkEventsManager : MonoBehaviourPun
{
    public static NetworkEventsManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    
    [PunRPC]
    public void RequestDamage(int targetViewID, float damage)
    {
        PhotonView target = PhotonView.Find(targetViewID);
        target.RPC("TakeDamage",target.Owner, damage);
    }
}
