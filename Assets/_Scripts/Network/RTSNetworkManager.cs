using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unit_spawner_prefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unit_spawner = Instantiate(unit_spawner_prefab, 
                                              conn.identity.transform.position, 
                                              conn.identity.transform.rotation);
        NetworkServer.Spawn(unit_spawner, conn);
    }
}
