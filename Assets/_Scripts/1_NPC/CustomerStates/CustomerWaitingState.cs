using UnityEngine;

public class CustomerWaitingState : CustomerBaseState
{
    public override void EnterState(CustomerBehaviour customer)
    {
    }

    public override void Update(CustomerBehaviour customer)
    {
        if (customer.IsOrderPlaced) return;
        if (!customer.Manager.IsSomeonePlacingOrder)
        {
            customer.TransitionToState(customer.mCustomerMovingState);
        }
    }
}
