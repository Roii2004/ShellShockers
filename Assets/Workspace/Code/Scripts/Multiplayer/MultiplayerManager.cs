using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    
    public Transform roomListContainer;
    public GameObject roomListItemPrefab;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        // This confirms the client is connected to the Photon master server
        Debug.Log("Connected to Photon Master Server.");
        
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {

        Debug.Log("Joined Lobby.");
        Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom("DrMikeGym", roomOptions);
        Debug.Log("Creating the sacred room of hypertrophy. Gains await. No cardio allowed.");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Step 1: Clear the old room buttons
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        // Step 2: Loop through the updated room list and spawn new buttons
        foreach (RoomInfo room in roomList)
        {
            GameObject item = Instantiate(roomListItemPrefab, roomListContainer);
            item.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;

            item.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
                Debug.Log("Attempting to join the temple of iron: " + room.Name);
            });
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entered the temple. Let the gains begin.");
        PhotonNetwork.LoadLevel("MortarScene");
    }
}
