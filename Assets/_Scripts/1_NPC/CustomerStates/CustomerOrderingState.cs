using UnityEngine;

public class CustomerOrderingState : CustomerBaseState
{
    public override void EnterState(CustomerBehaviour customer)
    {
        customer.Manager.IsSomeonePlacingOrder = true;
        customer.Restaurant.GetRandomOrder(customer);
    }

    public override void Update(CustomerBehaviour customer)
    {
        if(customer.IsOrderPlaced)
        {
            customer.Manager.IsSomeonePlacingOrder = false;
            customer.TransitionToState(customer.mCustomerMovingState);
        }
    }
}
