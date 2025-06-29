using Photon.Pun;
using UnityEngine;

public class MortarSceneManager : MonoBehaviour
{
    public Transform spawnPointPlayer1;
    public Transform spawnPointPlayer2;
    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // Spawn the networked player prefab for the current client
            // "PlayerPrefab" must be placed under a "Resources" folder in your Assets
            // The object must have a PhotonView component to be network-aware
            // This instantiates the object across the network and assigns ownership to this client
            
            Vector3 spawnOffset = (PhotonNetwork.LocalPlayer.ActorNumber == 1) ? spawnPointPlayer1.position : spawnPointPlayer2.position;
            GameObject playerMortar = PhotonNetwork.Instantiate("NetworkPrefabs/Mortar", spawnOffset, Quaternion.identity);
            string playerName = $"Mortar_Player{PhotonNetwork.LocalPlayer.ActorNumber}";
            playerMortar.name = playerName;

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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            int finalScore = 500; // Replace with actual game score logic later
            Debug.Log("P key pressed: sending score to server...");
            StartCoroutine(ScoreSender.SendScoreCoroutine(PlayerData.PlayerName, finalScore));
        }
    }
}
