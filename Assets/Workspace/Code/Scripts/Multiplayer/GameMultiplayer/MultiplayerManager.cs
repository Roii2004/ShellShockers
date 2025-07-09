using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static Action<string> NameConfirmed;
    public static Action LobbyJoined;
    public static Action RoomJoined;
    public static Action<List<RoomInfo>> RoomListUpdated;

    public SO_ServerData serverData;

    private string confirmedPlayerName = "";

    private void OnEnable()
    {
        UIManager.JoinLobbyRequested += HandleJoinLobbyRequested;
    }
    private void OnDisable()
    {
        UIManager.JoinLobbyRequested -= HandleJoinLobbyRequested;
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");

        PhotonNetwork.JoinLobby();
    }
    private void HandleJoinLobbyRequested()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Joining Lobby");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.LogWarning("Cannot join lobby, not connected to Photon.");
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined.");

        LobbyJoined?.Invoke();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListUpdated?.Invoke(roomList);
    }
    public override void OnJoinedRoom()
    {
        RoomJoined?.Invoke();

        PhotonNetwork.LoadLevel("MortarScene");
    }
    public void ConfirmPlayerName(string playerName)
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            confirmedPlayerName = playerName;
            PlayerData.PlayerName = confirmedPlayerName;

            // Inform UIManager that the name has been confirmed
            NameConfirmed?.Invoke(confirmedPlayerName);
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty.");
        }
    }
    public void CreateRoom(string roomNameInput)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Cannot create room: Photon is not connected and ready.");
            return;
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.LogError("Cannot create room: client has not joined the lobby yet.");
            return;
        }

        if (string.IsNullOrEmpty(confirmedPlayerName))
        {
            Debug.LogWarning("Player name not set.");
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput)
            ? "Room_" + UnityEngine.Random.Range(1000, 9999)
            : roomNameInput;

        PhotonNetwork.CreateRoom(roomName, new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        });
    }
}
