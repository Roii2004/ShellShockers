using UnityEngine;

[CreateAssetMenu(fileName = "ServerSettings", menuName = "Scriptable Objects/Server Settings")]
public class SO_ServerData : ScriptableObject
{
    [Header("Mode")]
    public bool isDevelopment = true;

    [Header("URLs")]
    public string developmentURL = "http://localhost:3000";
    public string productionURL = "https://yourgame.com";

    [Header("Routes")]
    public string topScores = "/topscores";
    public string addScore = "/addscore";

    public string BaseURL => isDevelopment ? developmentURL : productionURL;
}
