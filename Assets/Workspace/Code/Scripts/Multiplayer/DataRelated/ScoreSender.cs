using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
static class ScoreSender
{
    public static IEnumerator SendScoreCoroutine(string playerName, int score)
    {
        ScoreData data = new ScoreData(playerName, score);
        string jsonData = JsonUtility.ToJson(data);    
        
        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/addscore", "POST");
        
        System.Text.UTF8Encoding encodingData = new System.Text.UTF8Encoding();
        byte[] bodyRaw = encodingData.GetBytes(jsonData);
        
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score sent: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Error sending score: " + request.error);
        }
    }
}
