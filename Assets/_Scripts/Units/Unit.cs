using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unit_movement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    // On server
    public static event Action<Unit> onServerUnitSpawned;
    public static event Action<Unit> onServerUnitDespawned;

    // On client
    public static event Action<Unit> onAuthorityUnitSpawned;
    public static event Action<Unit> onAuthorityUnitDespawned;

    public UnitMovement getUnitMovement()
    {
        return unit_movement;
    }

    public Targeter getTargeter()
    {
        return targeter;
    }

    #region Server
    public override void OnStartServer()
    {
        onServerUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        onServerUnitDespawned?.Invoke(this);
    }
    #endregion

    #region Client
    [Client]
    public void select()
    {
        if (!hasAuthority)
        {
            return;
        }

        onSelected?.Invoke();
    }

    [Client]
    public void deselect()
    {
        if (!hasAuthority)
        {
            return;
        }

        onDeselected?.Invoke();
    }

    [Client]
    public override void OnStartClient()
    {
        if(!isClientOnly || !hasAuthority)
        {
            return;
        }

        onAuthorityUnitSpawned?.Invoke(this);
    }

    [Client]
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        onAuthorityUnitDespawned?.Invoke(this);
    }
    #endregion
}
