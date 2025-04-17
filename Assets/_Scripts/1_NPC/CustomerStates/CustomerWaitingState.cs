using UnityEngine;

public class CustomerWaitingState : NPCBaseState
{
    CustomerBehaviour customer;
    public override void EnterState<T>(T npc)
    {
        customer = npc as CustomerBehaviour;
    }

    public override void Update()
    {
        if (customer.IsOrderFulfilled)
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
