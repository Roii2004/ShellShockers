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
    public void RequestDamage(int targetViewID, float amount)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonView targetView = PhotonView.Find(targetViewID);
        if (targetView != null)
        {
            targetView.RPC("TakeDamage", targetView.Owner, amount);
        }
    }
}
