using UnityEngine;

public class FighterEmployeeIdleState : NPCBaseState
{
    FighterEmployeeBehaviour employee;
    public override void EnterState<T>(T npc)
    {
        employee = npc as FighterEmployeeBehaviour;
        employee.Animator.SetBool(employee.m_HashAttacking, false);
        employee.Animator.SetBool(employee.m_HashMoving, false);
    }

    public override void Update()
    {
        
    }
}
