using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unit_base_prefab = null;
    [SerializeField] private GameOverHandler gameover_handler_prefab = null;

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

    public List<RTSPlayer> players { get; } = new List<RTSPlayer>();
    private bool is_game_progressing = false;

    #region Server
    public override void OnServerConnect(NetworkConnection conn)
    {
        // New player cannot connect when the game is progressing
        if (is_game_progressing)
        {
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        players.Clear();
        is_game_progressing = false;
    }

    public void startGame()
    {
        if(players.Count < 2)
        {
            return;
        }

        is_game_progressing = true;
        ServerChangeScene("Map01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        player.setTeamColor(new Color(Random.Range(0f, 1f),
                                      Random.Range(0f, 1f),
                                      Random.Range(0f, 1f)));

        players.Add(player);
        player.setDisplayName($"Player {players.Count}");

        player.setPartyOwner(players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            GameOverHandler handler = Instantiate(gameover_handler_prefab);
            NetworkServer.Spawn(handler.gameObject);

            foreach(RTSPlayer player in players)
            {
                GameObject instance = Instantiate(unit_base_prefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(instance, player.connectionToClient);
            }
        }
    }
    #endregion

    #region Client
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        onClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        onClientDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        players.Clear();
    }
    #endregion
}
