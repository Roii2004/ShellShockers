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
    
    public void RequestExplosion(int shellViewID, Vector3 position)
    {
        photonView.RPC("ExecuteExplosion", RpcTarget.MasterClient, shellViewID, position);
    }
    
    [PunRPC]
    public void ExecuteExplosion(int shellViewID, Vector3 position)
    {
        PhotonView shellView = PhotonView.Find(shellViewID);
        if (shellView != null && shellView.TryGetComponent(out OrdnanceBaseBehaviour ordnance))
        {
            ordnance.ForceExplode(); // This is a method that just runs ApplyExplosionLogic()
        }
    }
    
    [PunRPC]
    public void RequestDamage(int targetViewID, float damage)
    {
        PhotonView target = PhotonView.Find(targetViewID);
        target.RPC("TakeDamage",target.Owner, damage);
    }
}
