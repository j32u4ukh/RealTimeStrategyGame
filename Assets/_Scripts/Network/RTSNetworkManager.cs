using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unit_spawner_prefab = null;
    [SerializeField] private GameOverHandler gameover_handler_prefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unit_spawner = Instantiate(unit_spawner_prefab, 
                                              conn.identity.transform.position, 
                                              conn.identity.transform.rotation);
        NetworkServer.Spawn(unit_spawner, conn);
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
