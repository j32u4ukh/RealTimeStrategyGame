using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unit_prefab = null;
    [SerializeField] private Transform spawn_point;

    #region Server
    [Command]
    private void cmdSpawnUnit()
    {
        GameObject unit = Instantiate(unit_prefab, spawn_point.position, spawn_point.rotation);

        // connectionToClient: 當前連線的 Client
        NetworkServer.Spawn(unit, connectionToClient); 
    }

    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            if (!hasAuthority)
            {
                return;
            }

            cmdSpawnUnit();
        }
    }


    #endregion
}
