using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> onClientGameOver;
    private List<UnitBase> bases = new List<UnitBase>();

    #region Server
    public override void OnStartServer()
    {
        UnitBase.onServerBaseSpawned += handleServerBaseSpawned;
        UnitBase.onServerBaseDespawned += handleServerBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.onServerBaseSpawned -= handleServerBaseSpawned;
        UnitBase.onServerBaseDespawned -= handleServerBaseDespawned;
    }

    [Server]
    private void handleServerBaseSpawned(UnitBase unitbase)
    {
        bases.Add(unitbase);
    }

    [Server]
    private void handleServerBaseDespawned(UnitBase unitbase)
    {
        bases.Remove(unitbase);

        if(bases.Count == 1)
        {
            int winner_id = bases[0].connectionToClient.connectionId;
            rpcGameOver(winner: $"Player {winner_id}");
        }
    }
    #endregion

    #region Client
    [ClientRpc]
    private void rpcGameOver(string winner)
    {
        onClientGameOver?.Invoke(winner);
    }
    #endregion
}
