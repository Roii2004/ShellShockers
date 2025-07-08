using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    // These events will notify UIManager when networking events occur
    public static Action<string> NameConfirmed;
    public static Action LobbyJoined;
    public static Action RoomJoined;
    public static Action<List<RoomInfo>> RoomListUpdated;

    public SO_ServerData serverData;

    private string confirmedPlayerName = "";

    new void OnEnable()
    {
        // Listen for UIManager telling us to join the lobby
        UIManager.JoinLobbyRequested += HandleJoinLobbyRequested;
    }

    new void OnDisable()
    {
        UIManager.JoinLobbyRequested -= HandleJoinLobbyRequested;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master.");
        // Optional: you could auto-join lobby here if desired
    }

    // Called when UIManager requests to join the Photon lobby
    private void HandleJoinLobbyRequested()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
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

        // Inform UIManager that we've joined the lobby so it can update UI
        LobbyJoined?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Send the updated room list to UIManager so it can display it
        RoomListUpdated?.Invoke(roomList);
    }

    public override void OnJoinedRoom()
    {
        // Notify UIManager or others that a room was successfully joined
        RoomJoined?.Invoke();

        // Load the actual multiplayer scene
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
            Debug.LogError("Photon not ready. Wait for OnConnectedToMaster.");
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

    public IEnumerator FetchScores(Action<List<ScoreEntry>> onScoresFetched)
    {
        UnityWebRequest request = UnityWebRequest.Get(serverData.BaseURL + serverData.topScores);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch scores: " + request.error);
            onScoresFetched?.Invoke(null);
            yield break;
        }

        string wrappedJson = "{\"scores\":" + request.downloadHandler.text + "}";
        ScoreEntryList scoreList = JsonUtility.FromJson<ScoreEntryList>(wrappedJson);
        onScoresFetched?.Invoke(scoreList.scores);
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class ScoreEntryList
    {
        public List<ScoreEntry> scores;
    }
}
