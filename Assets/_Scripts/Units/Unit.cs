using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    #region Server

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
    #endregion
}
