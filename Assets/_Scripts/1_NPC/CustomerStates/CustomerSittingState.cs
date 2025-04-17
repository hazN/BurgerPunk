using UnityEngine;

public class CustomerSittingState : NPCBaseState
{
    public override void EnterState<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
        customer.Animator.SetBool(customer.m_HashSit, true);
    }

    public override void Update<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
    }
}
