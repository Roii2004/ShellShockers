using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // Used to reload scoreboards 
    public static Action JoinLobbyRequested;

    [Header("Server Info")]
    public SO_ServerData serverData;

    [Header("Buttons")]
    public Button confirmNameButton;
    public Button createRoomButton;
    public Button returnToRoomListButton;
    public Button seeScoreboardButton;

    [Header("Input Fields")]
    public TMP_InputField playerNameInput;
    public TMP_InputField roomNameInput;

    [Header("UI Containers")]
    public Transform UIListContainer;
    public GameObject roomListItemPrefab;
    public GameObject scoreboardItemPrefab;

    [Header("UI Groups")]
    public List<GameObject> preNameUIObjects;
    public List<GameObject> postNameUIObjects;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Listen to events sent from MultiplayerManager
        MultiplayerManager.NameConfirmed += HandleNameConfirmed;
        MultiplayerManager.RoomListUpdated += UpdateRoomList;
    }

    private void OnDisable()
    {
        MultiplayerManager.NameConfirmed -= HandleNameConfirmed;
        MultiplayerManager.RoomListUpdated -= UpdateRoomList;
    }

    private void Start()
    {
        confirmNameButton.onClick.AddListener(() =>
        {
            FindObjectOfType<MultiplayerManager>().ConfirmPlayerName(playerNameInput.text);
        });

        createRoomButton.onClick.AddListener(() =>
        {
            FindObjectOfType<MultiplayerManager>().CreateRoom(roomNameInput.text);
        });

        returnToRoomListButton.onClick.AddListener(HandleReturnToRoomListClicked);
        returnToRoomListButton.gameObject.SetActive(false);

        seeScoreboardButton.onClick.AddListener(OnSeeScoreboardClicked);

        foreach (var go in preNameUIObjects) go.SetActive(true);
        foreach (var go in postNameUIObjects) go.SetActive(false);
    }
    public void HandleNameConfirmed(string playerName)
    {
        Debug.Log($"Name confirmed: {playerName}");

        createRoomButton.interactable = true;

        foreach (var go in preNameUIObjects) go.SetActive(false);
        foreach (var go in postNameUIObjects) go.SetActive(true);
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
    private void HandleReturnToRoomListClicked()
    {
        returnToRoomListButton.gameObject.SetActive(false);

        foreach (Transform child in UIListContainer)
        {
            Destroy(child.gameObject);
        }

        JoinLobbyRequested?.Invoke();
    }
    private IEnumerator FetchAndDisplayScores()
    {
        string url = serverData.BaseURL + serverData.topScores;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch scores: " + request.error);
            yield break;
        }

        string wrappedJson = "{\"scores\":" + request.downloadHandler.text + "}";
        ScoreEntryList scoreList = JsonUtility.FromJson<ScoreEntryList>(wrappedJson);

        foreach (ScoreEntry entry in scoreList.scores)
        {
            GameObject row = Instantiate(scoreboardItemPrefab, UIListContainer);
            PlayerNameScoreButtonUI ui = row.GetComponent<PlayerNameScoreButtonUI>();
            ui.playerNameText.text = entry.playerName;
            ui.playerScoreText.text = entry.score.ToString();
        }
    }
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in UIListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible) continue;

            GameObject item = Instantiate(roomListItemPrefab, UIListContainer);
            var ui = item.GetComponent<RoomListItemUI>();
            ui.SetRoomInfo(room);

            // Directly call Photon here is OK, but can be refactored similarly if desired
            ui.SetJoinAction(() => Photon.Pun.PhotonNetwork.JoinRoom(room.Name));
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
