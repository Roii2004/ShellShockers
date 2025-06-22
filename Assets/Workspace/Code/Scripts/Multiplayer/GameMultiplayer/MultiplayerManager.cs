using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField roomNameInput;
    public Button createRoomButton;

    public Transform roomListContainer;
    public GameObject roomListItemPrefab;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        createRoomButton.onClick.AddListener(CreateRoom);
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

    public void CreateRoom()
    {
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
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate list
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
                continue;

            GameObject item = Instantiate(roomListItemPrefab, roomListContainer);
            var ui = item.GetComponent<RoomListItemUI>();
            ui.SetRoomInfo(room);
            ui.SetJoinAction(() => PhotonNetwork.JoinRoom(room.Name));
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MortarScene");
    }
}
