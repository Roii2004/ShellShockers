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
            
            Vector3 spawnOffset = new Vector3(PhotonNetwork.CurrentRoom.PlayerCount * 2f, 0, 0);
            GameObject playerMortar = PhotonNetwork.Instantiate("NetworkPrefabs/Mortar", spawnOffset, Quaternion.identity);

            PhotonView pv = playerMortar.GetComponent<PhotonView>();
            if (pv != null)
            {
                Debug.Log($"Spawned mortar. ViewID: {pv.ViewID}, IsMine: {pv.IsMine}, Owner: {pv.Owner.NickName}");
            }
            else
            {
                Debug.LogWarning("Spawned mortar does not have a PhotonView!");
            }

            Debug.Log("Entered the temple. Let the gains begin.");
        }
    }
}
