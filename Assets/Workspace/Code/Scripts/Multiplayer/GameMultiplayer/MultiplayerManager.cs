using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("Button UI References")]
    public Button confirmNameButton;
    public Button createRoomButton;

    [Header("Input Fields UI References")]
    public TMP_InputField playerNameInput;
    public TMP_InputField roomNameInput;

    [Header("UI Other References")]
    public Transform UIListContainer;
    public GameObject roomListItemPrefab;
    public GameObject scoreboardItemPrefab; 
    public GameObject scrollViewGO;

    [Header("UI Manager List")]
    public List<GameObject> preNameUIObjects;
    public List<GameObject> postNameUIObjects;
    
    private string confirmedPlayerName = "";

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        createRoomButton.interactable = false;
        createRoomButton.onClick.AddListener(CreateRoom);
        confirmNameButton.onClick.AddListener(ConfirmPlayerName);
        
        foreach (var go in preNameUIObjects) go.SetActive(true);
        foreach (var go in postNameUIObjects) go.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");
        // Now user manually clicks "Create Room"
    }

    private void ConfirmPlayerName()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            confirmedPlayerName = playerNameInput.text;
            PlayerData.PlayerName = confirmedPlayerName;
            Debug.Log(PlayerData.PlayerName);
            createRoomButton.interactable = true;
            Debug.Log("Player name confirmed: " + confirmedPlayerName);

            // Hide all objects in the pre-name UI group
            foreach (var go in preNameUIObjects) go.SetActive(false);
            // Show all objects in the post-name UI group
            foreach (var go in postNameUIObjects) go.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty.");
        }
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(confirmedPlayerName))
        {
            Debug.LogWarning("Player name not set.");
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? "Room_" + UnityEngine.Random.Range(1000, 9999) : roomNameInput.text;

        PhotonNetwork.CreateRoom(roomName, new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        });
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear old UI
        foreach (Transform child in UIListContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate list
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
                continue;

            GameObject item = Instantiate(roomListItemPrefab, UIListContainer);
            var ui = item.GetComponent<RoomListItemUI>();
            ui.SetRoomInfo(room);
            ui.SetJoinAction(() => PhotonNetwork.JoinRoom(room.Name));
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MortarScene");
    }

    public void OnSeeScoreboardClicked()
    {
        foreach (Transform child in UIListContainer)
        {
            Destroy(child.gameObject);
        }

        // Example placeholder data
        for (int i = 0; i < 10; i++)
        {
            GameObject row = Instantiate(scoreboardItemPrefab, UIListContainer);
            row.GetComponentInChildren<TMPro.TMP_Text>().text = $"Player_{i + 1} - {1000 - i * 100}";
        }
    }
}
