using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class RoomListItemUI : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    public Button joinButton;
    
    public void SetRoomInfo(RoomInfo info)
    {
        roomNameText.text = info.Name;
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
    }

    // This sets the function that should be called when the Join button is clicked
    public void SetJoinAction(System.Action joinAction)
    {
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => joinAction.Invoke());
    }
}
