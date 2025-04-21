using UnityEngine;
using UnityEngine.AI;

public abstract class NPCBaseState
{
    /// <summary>
    /// When npc enters the state
    /// </summary>
    /// <param name="npc"></param>
    public abstract void EnterState<T>(T npc);

    /// <summary>
    /// Frame Update
    /// </summary>
    /// <param name="npc"></param>
    public abstract void Update();
}

static class Helper
{
    public static bool HaveReached(NavMeshAgent navMeshAgent)
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                return true;
                /*if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                }*/
            }
        }
        return false;
    }
}
