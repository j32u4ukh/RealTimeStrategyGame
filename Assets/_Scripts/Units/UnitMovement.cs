using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    private Camera main_canera;

    [Server]
    public void move(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, 0.01f);
    }

    #region Server
    [Command]
    private void cmdMove(Vector3 destination)
    {
        if(NavMesh.SamplePosition(sourcePosition: destination, 
                                  hit: out NavMeshHit hit, 
                                  maxDistance: 1f, 
                                  areaMask: NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        main_canera = Camera.main;
    }

    /// <summary>
    /// [ClientCallback]: �קK Server ����o�q�{��
    /// </summary>
    [ClientCallback]
    private void Update()
    {
        // �T�O�u����e���a����o�Ө禡
        if (!hasAuthority)
        {
            return;
        }

        // �Y���U�ƹ��k��
        // Input.GetMouseButtonDown(1) -> Mouse.current.rightButton.wasPressedThisFrame
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Input.mousePosition -> Mouse.current.position.ReadValue()
            Ray ray = main_canera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                cmdMove(hit.point);
            }
        }
    }
    #endregion
}
