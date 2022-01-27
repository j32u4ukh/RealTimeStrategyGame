using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chasing_distance = 10f;
    private Targetable target;

    #region Server
    // [ServerCallback]: 只在 Server 上執行，避免被 Client 端執行
    [ServerCallback]
    private void Update()
    {
        target = targeter.getTarget();

        // Chasing movement
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chasing_distance * chasing_distance)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }

        // Normal movement
        else
        {
            if (!agent.hasPath)
            {
                return;
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
            }
        }
    }

    [Command]
    public void cmdMove(Vector3 destination)
    {
        targeter.clearTarget();

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
