using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;

    #region Server
    // [ServerCallback]: �u�b Server �W����A�קK�Q Client �ݰ���
    [ServerCallback]
    private void Update()
    {
        if (!agent.hasPath)
        {
            return;
        }

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
        }
    }

    [Command]
    public void cmdMove(Vector3 destination)
    {
        targeter.clearTarget();

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
