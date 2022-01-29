using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject building_preview = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    // On server
    public static event Action<Building> onServerBuildingSpawned;
    public static event Action<Building> onServerBuildingDespawned;

    // On client
    public static event Action<Building> onAuthorityBuildingSpawned;
    public static event Action<Building> onAuthorityBuildingDespawned;

    public GameObject getBuildingPreview()
    {
        return building_preview;
    }

    public Sprite getIcon()
    {
        return icon;
    }

    public int getId()
    {
        return id;
    }

    public int getPrice()
    {
        return price;
    }

    #region Server
    public override void OnStartServer()
    {
        onServerBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        onServerBuildingDespawned?.Invoke(this);
    }
    #endregion

    #region Client
    [Client]
    public override void OnStartAuthority()
    {
        onAuthorityBuildingSpawned?.Invoke(this);
    }

    // Building 被消滅或 Server 停止運行時，觸發 OnStopClient
    [Client]
    public override void OnStopAuthority()
    {
        if (!hasAuthority)
        {
            return;
        }

        onAuthorityBuildingDespawned?.Invoke(this);
    }
    #endregion
}
