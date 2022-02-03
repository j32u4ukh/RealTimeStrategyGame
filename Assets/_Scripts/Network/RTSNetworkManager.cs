using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unit_spawner_prefab = null;
    [SerializeField] private GameOverHandler gameover_handler_prefab = null;

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        player.setTeamColor(new Color(Random.Range(0f, 1f), 
                                      Random.Range(0f, 1f), 
                                      Random.Range(0f, 1f)));

        //GameObject unit_spawner = Instantiate(unit_spawner_prefab, 
        //                                      conn.identity.transform.position, 
        //                                      conn.identity.transform.rotation);
        //NetworkServer.Spawn(unit_spawner, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            GameOverHandler handler = Instantiate(gameover_handler_prefab);
            NetworkServer.Spawn(handler.gameObject);
        }
    }
}
