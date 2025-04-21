using UnityEngine;

public class CustomerSittingState : NPCBaseState
{
    CustomerBehaviour customer;
    public override void EnterState<T>(T npc)
    {
        customer = npc as CustomerBehaviour;
        customer.Animator.SetBool(customer.m_HashSit, true);
    }

    public override void Update()
    {
        if (customer != null)
        {
            if (customer.IsOrderFulfilled)
            {
                // customer eats and leave
            }
        }
    }
}
