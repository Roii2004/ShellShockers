using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("Button UI References")]
    public Button confirmNameButton;
    public Button createRoomButton;
    public Button returnToRoomListButton;

    [Header("Input Fields UI References")]
    public TMP_InputField playerNameInput;
    public TMP_InputField roomNameInput;

    [Header("UI Other References")]
    public Transform UIListContainer;
    public GameObject roomListItemPrefab;
    public GameObject scoreboardItemPrefab; 

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
        returnToRoomListButton.gameObject.SetActive(false);
        returnToRoomListButton.onClick.AddListener(OnReturnToRoomListClicked);
        
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
        createRoomButton.interactable = true;
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

        StartCoroutine(FetchAndDisplayScores());
        returnToRoomListButton.gameObject.SetActive(true);
    }

    private void OnReturnToRoomListClicked()
    {
        returnToRoomListButton.gameObject.SetActive(false);

        // Clear current UI
        foreach (Transform child in UIListContainer)
        {
            Destroy(child.gameObject);
        }

        // Trigger room list refresh manually
        PhotonNetwork.JoinLobby();
    }

    private IEnumerator FetchAndDisplayScores()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/topscores");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch scores: " + request.error);
            yield break;
        }

        string wrappedJson = "{\"scores\":" + request.downloadHandler.text + "}";
        
        //function takes a JSON string and parses it into an object of type ScoreEntryList .
        ScoreEntryList scoreList = JsonUtility.FromJson<ScoreEntryList>(wrappedJson);

        foreach (ScoreEntry entry in scoreList.scores)
        {
            GameObject row = Instantiate(scoreboardItemPrefab, UIListContainer);
            PlayerNameScoreButtonUI ui = row.GetComponent<PlayerNameScoreButtonUI>();
            ui.playerNameText.text = entry.playerName;
            ui.playerScoreText.text = entry.score.ToString();
        }
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
