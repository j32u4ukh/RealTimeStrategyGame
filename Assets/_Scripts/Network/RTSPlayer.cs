using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] building_templates = new Building[0];

    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public List<Unit> getUnits()
    {
        return units;
    }

    public List<Building> getBuildings()
    {
        return buildings;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.onServerUnitSpawned += handleServerUnitSpawned;
        Unit.onServerUnitDespawned += handleServerUnitDespawned;
        Building.onServerBuildingSpawned += handleServerBuildingSpawned;
        Building.onServerBuildingDespawned += handleServerBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.onServerUnitSpawned -= handleServerUnitSpawned;
        Unit.onServerUnitDespawned -= handleServerUnitDespawned;
        Building.onServerBuildingSpawned -= handleServerBuildingSpawned;
        Building.onServerBuildingDespawned -= handleServerBuildingDespawned;
    }

    [Command]
    public void cmdTryPlaceBuilding(int building_id, Vector3 location)
    {
        Building building = null;

        foreach(Building template in building_templates)
        {
            if (template.getId().Equals(building_id))
            {
                building = template;
                break;
            }
        }

        if(building != null)
        {
            GameObject building_instance = Instantiate(building.gameObject, location, building.transform.rotation);
            NetworkServer.Spawn(building_instance, connectionToClient);
        }
    }

    private void handleServerUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Add(unit);
    }

    private void handleServerUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Remove(unit);
    }

    private void handleServerBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        buildings.Add(building);
    }

    private void handleServerBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        buildings.Remove(building);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        // server has been started
        if (NetworkServer.active)
        {
            return;
        }

        Unit.onAuthorityUnitSpawned += handleAuthorityUnitSpawned;
        Unit.onAuthorityUnitDespawned += handleAuthorityUnitDespawned;
        Building.onAuthorityBuildingSpawned += handleAuthorityBuildingSpawned;        
        Building.onAuthorityBuildingDespawned += handleAuthorityBuildingDespawned;
    }

    public override void OnStopClient()
    {
        // 已知 Client 才能觸發相關事件，因此無須再次檢查 connectionId，但仍需檢查使否是所有者的 Client
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        Unit.onAuthorityUnitSpawned -= handleAuthorityUnitSpawned;
        Unit.onAuthorityUnitDespawned -= handleAuthorityUnitDespawned;
        Building.onAuthorityBuildingSpawned -= handleAuthorityBuildingSpawned;
        Building.onAuthorityBuildingDespawned -= handleAuthorityBuildingDespawned;
    }

    private void handleAuthorityUnitSpawned(Unit unit)
    {
        units.Add(unit); 
    }

    private void handleAuthorityUnitDespawned(Unit unit)
    {
        units.Remove(unit);
    }

    private void handleAuthorityBuildingSpawned(Building building)
    {
        buildings.Add(building);
    }

    private void handleAuthorityBuildingDespawned(Building building)
    {
        buildings.Remove(building);
    }
    #endregion
}
