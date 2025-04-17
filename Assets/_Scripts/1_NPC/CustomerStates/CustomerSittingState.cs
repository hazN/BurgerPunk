using UnityEngine;

public class CustomerSittingState : CustomerBaseState
{
    public override void EnterState(CustomerBehaviour customer)
    {
        customer.Animator.SetBool(customer.m_HashSit, true);
    }

    public override void Update(CustomerBehaviour customer)
    {

    }
}
