using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameover_display_parent = null;
    [SerializeField] private TMP_Text winner_name = null;

    // Start is called before the first frame update
    void Start()
    {
        GameOverHandler.onClientGameOver += handleClientGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.onClientGameOver -= handleClientGameOver;
    }

    private void handleClientGameOver(string winner)
    {
        winner_name.text = $"{winner} Has Won!";
        gameover_display_parent.SetActive(true);
    }

    public void leaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // stop client
            NetworkManager.singleton.StopClient();
        }
    }
}
