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
    /// [ClientCallback]: 避免 Server 執行這段程式
    /// </summary>
    [ClientCallback]
    private void Update()
    {
        // 確保只有當前玩家執行這個函式
        if (!hasAuthority)
        {
            return;
        }

        // 若按下滑鼠右鍵
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
