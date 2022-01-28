using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
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
        // �w�� Client �~��Ĳ�o�����ƥ�A�]���L���A���ˬd connectionId�A�������ˬd�ϧ_�O�Ҧ��̪� Client
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
