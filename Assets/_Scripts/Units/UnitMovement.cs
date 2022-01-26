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
        // �ˬd destination �O�_�i�H�L�h�A�êu�۫e�����u�W�i����I(�i��i���I)
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
