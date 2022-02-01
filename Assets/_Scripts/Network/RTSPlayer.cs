using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform camera_transform = null;
    [SerializeField] private LayerMask building_block_layer = new LayerMask();
    [SerializeField] private Building[] building_templates = new Building[0];
    [SerializeField] private float building_range_limit = 5f;

    [SyncVar(hook = nameof(handleClientResourcesUpdated))] private int resources = 500;

    public event Action<int> onClientResourcesUpdated;

    private Color team_color = new Color();
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public Transform getCameraTransform()
    {
        return camera_transform;
    }

    public Color getTeamColor()
    {
        return team_color;
    }

    public List<Unit> getUnits()
    {
        return units;
    }

    public List<Building> getBuildings()
    {
        return buildings;
    }

    public int getResources()
    {
        return resources;
    }

    /// <summary>
    /// TODO: 不知為何 Physics.CheckBox 一直被觸發，但明明沒有重疊
    /// </summary>
    /// <param name="building_collider"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public bool canPlaceBuilding(BoxCollider building_collider, Vector3 location)
    {
        if (Physics.CheckBox(location + building_collider.center,
                            building_collider.size / 2,
                            Quaternion.identity,
                            building_block_layer))
        {
            Debug.Log($"false, Physics.CheckBox");
            return false;
        }

        foreach (Building building in buildings)
        {
            if ((location - building.transform.position).sqrMagnitude <= (building_range_limit * building_range_limit))
            {
                Debug.Log($"location: {location}, building: {building.transform.position}");
                return true;
            }
        }

        Debug.Log($"false, too far");
        return false;
    }

    #region Server
    [Server]
    public void setResources(int resources)
    {
        this.resources = resources;
    }

    [Server]
    public void setTeamColor(Color color)
    {
        team_color = color;
    }

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
        Building building_to_place = null;

        foreach(Building template in building_templates)
        {
            if (template.getId().Equals(building_id))
            {
                building_to_place = template;
                break;
            }
        }

        if(building_to_place == null)
        {
            return;
        }

        if(resources < building_to_place.getPrice())
        {
            return;
        }

        BoxCollider building_collider = building_to_place.GetComponent <BoxCollider>();
   
        if (canPlaceBuilding(building_collider, location))
        {
            GameObject building_instance = Instantiate(building_to_place.gameObject, location, building_to_place.transform.rotation);
            NetworkServer.Spawn(building_instance, connectionToClient);
            setResources(resources - building_to_place.getPrice());
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

    private void handleClientResourcesUpdated(int origin_value, int new_value)
    {
        //resources = new_value;
        onClientResourcesUpdated?.Invoke(new_value);
    }
    #endregion
}
