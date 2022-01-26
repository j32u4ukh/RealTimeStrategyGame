using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    #region Server
    [Command]
    public void cmdMove(Vector3 destination)
    {
        // 檢查 destination 是否可以過去，並沿著前網路線上進行採點(可行進的點)
        if (NavMesh.SamplePosition(sourcePosition: destination, 
                                  hit: out NavMeshHit hit, 
                                  maxDistance: 1f, 
                                  areaMask: NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    #endregion
}
