using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobby_ui = null;


    private void Start()
    {
        RTSNetworkManager.onClientConnected += handleClientConnected;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.onClientConnected -= handleClientConnected;
    }

    private void handleClientConnected()
    {
        lobby_ui.SetActive(true);
    }

    public void leaveLobby()
    {
        // Run as a aserver and as a client -> host
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }

        // Client
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }
}
