using UnityEngine;
using UnityEngine.AI;

public class FighterEmployeeMoveState : NPCBaseState
{
    FighterEmployeeBehaviour employee;
    public override void EnterState<T>(T npc)
    {
        employee = npc as FighterEmployeeBehaviour;
        employee.Animator.SetBool(employee.m_HashAttacking, false);
        employee.Animator.SetBool(employee.m_HashMoving, true);
        employee.NavMeshAgent.speed = 1f;
    }

    public override void Update()
    {
        if (Helper.HaveReached(employee.NavMeshAgent))
        {
            employee.Animator.SetBool(employee.m_HashMoving, false);
            employee.TransitionToState(employee.mEmployeeAttackState);
        }
    }
}