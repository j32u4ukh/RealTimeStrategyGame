using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobby_ui = null;
    [SerializeField] private Button start_game_button = null;


    private void Start()
    {
        RTSNetworkManager.onClientConnected += handleClientConnected;
        RTSPlayer.onAuthorityPartyOwnerStateUpdated += handleAuthorityPartyOwnerStateUpdated;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.onClientConnected -= handleClientConnected;
    }

    private void handleClientConnected()
    {
        lobby_ui.SetActive(true);
    }

    private void handleAuthorityPartyOwnerStateUpdated(bool state)
    {
        start_game_button.gameObject.SetActive(state);
    }

    public void startGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().cmdStartGame();
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
