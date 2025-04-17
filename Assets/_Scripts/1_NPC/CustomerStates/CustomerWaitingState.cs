using UnityEngine;

public class CustomerWaitingState : NPCBaseState
{
    public override void EnterState<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
    }

    public override void Update<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
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
