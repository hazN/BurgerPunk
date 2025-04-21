using UnityEngine;

public class FighterEmployeeAttackState : NPCBaseState
{
    FighterEmployeeBehaviour employee;
    public override void EnterState<T>(T npc)
    {
        employee = npc as FighterEmployeeBehaviour;
        employee.Animator.SetBool(employee.m_HashAttacking, true);
    }

    public override void Update()
    {
    }
}