using UnityEngine;

public class CustomerOrderingState : CustomerBaseState
{
    public bool OrderFinished = false;
    public override void EnterState(CustomerBehaviour customer)
    {
        customer.Animator.SetBool(customer.m_HashMove, true);
        customer.Animator.SetTrigger(customer.m_HashOrder1);
    }

    public override void Update(CustomerBehaviour customer)
    {
        if(OrderFinished)
        {
           customer.TransitionToState(customer.mCustomerMovingState);
        }
    }
}
