using Photon.Pun;
using UnityEngine;

public class MortarSceneManager : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // Spawn the networked player prefab for the current client
            // "PlayerPrefab" must be placed under a "Resources" folder in your Assets
            // The object must have a PhotonView component to be network-aware
            // This instantiates the object across the network and assigns ownership to this client
            
            PhotonNetwork.Instantiate("NetworkPrefabs/Mortar", new Vector3(0, 0, 0), Quaternion.identity);
            Debug.Log("Entered the temple. Let the gains begin.");
        }
    }

    void Update()
    {
        
    }
}
