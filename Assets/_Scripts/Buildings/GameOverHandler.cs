using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
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
            Debug.Log("Game Over");
        }
    }
    #endregion

    #region Client
    #endregion
}
