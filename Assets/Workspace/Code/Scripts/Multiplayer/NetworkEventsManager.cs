using UnityEngine;
using Photon.Pun;
public class NetworkEventsManager : MonoBehaviourPun
{
    public static NetworkEventsManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
