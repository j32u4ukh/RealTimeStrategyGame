using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> units = new List<Unit>();

    public List<Unit> getUnits()
    {
        return units;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.onServerUnitSpawned += handleServerUnitSpawned;
        Unit.onServerUnitDespawned += handleServerUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.onServerUnitSpawned -= handleServerUnitSpawned;
        Unit.onServerUnitDespawned -= handleServerUnitDespawned;
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

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (!isClientOnly)
        {
            return;
        }

        Unit.onAuthorityUnitSpawned += handleAuthorityUnitSpawned;
        Unit.onAuthorityUnitDespawned += handleAuthorityUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly)
        {
            return;
        }

        Unit.onAuthorityUnitSpawned -= handleAuthorityUnitSpawned;
        Unit.onAuthorityUnitDespawned -= handleAuthorityUnitDespawned;
    }

    private void handleAuthorityUnitSpawned(Unit unit)
    {
        // �w�� Client �~��Ĳ�o�����ƥ�A�]���L���A���ˬd connectionId�A�������ˬd�ϧ_�O�Ҧ��̪� Client
        if (hasAuthority)
        {
            units.Add(unit);
        }
    }

    private void handleAuthorityUnitDespawned(Unit unit)
    {
        if (hasAuthority)
        {
            units.Remove(unit);
        }
    }

    #endregion
}
