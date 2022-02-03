using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobby_ui = null;
    [SerializeField] private Button start_game_button = null;
    [SerializeField] private TMP_Text[] player_name_texts = new TMP_Text[4];


    private void Start()
    {
        RTSNetworkManager.onClientConnected += handleClientConnected;
        RTSPlayer.onAuthorityPartyOwnerStateUpdated += handleAuthorityPartyOwnerStateUpdated;
        RTSPlayer.onClientInfoUpdated += handleClientInfoUpdated;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.onClientConnected -= handleClientConnected;
        RTSPlayer.onAuthorityPartyOwnerStateUpdated -= handleAuthorityPartyOwnerStateUpdated;
        RTSPlayer.onClientInfoUpdated -= handleClientInfoUpdated;
    }

    private void handleClientConnected()
    {
        lobby_ui.SetActive(true);
    }

    private void handleAuthorityPartyOwnerStateUpdated(bool state)
    {
        start_game_button.gameObject.SetActive(state);
    }

    private void handleClientInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).players;

        for(int i = 0; i < players.Count; i++)
        {
            player_name_texts[i].text = players[i].getDisplayName();
        }

        for(int i = players.Count; i < player_name_texts.Length; i++)
        {
            player_name_texts[i].text = "Waiting For Player...";
        }

        start_game_button.interactable = players.Count >= 2;
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
