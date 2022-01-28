using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<int> onServerPlayerDie;
    public static event Action<UnitBase> onServerBaseSpawned;
    public static event Action<UnitBase> onServerBaseDespawned;

    #region Server
    public override void OnStartServer()
    {
        health.onServerDie += handleServerDie;
        onServerBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        onServerBaseDespawned?.Invoke(this);
        health.onServerDie -= handleServerDie;
    }

    [Server]
    private void handleServerDie()
    {
        onServerPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    #endregion
}
