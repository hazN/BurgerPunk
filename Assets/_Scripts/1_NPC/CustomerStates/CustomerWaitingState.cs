using UnityEngine;

public class CustomerWaitingState : CustomerBaseState
{
    public override void EnterState(CustomerBehaviour customer)
    {
    }

    public override void Update(CustomerBehaviour customer)
    {
        if(customer.IsOrderFulfilled)
        {
            customer.Restaurant.OrderFulfilled(customer);
            customer.Restaurant.AssignTask();
            return;
        }
        if (!customer.IsOrderPlaced && !customer.Manager.IsSomeonePlacingOrder)
        {
            customer.TransitionToState(customer.mCustomerMovingState);
        }
    }
}
