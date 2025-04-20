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
            Restaurant.Instance.OrderFulfilled(customer);
            Restaurant.Instance.AssignTask();
            return;
        }
        if (!customer.IsOrderPlaced && !CustomerManager.Instance.IsSomeonePlacingOrder)
        {
            customer.TransitionToState(customer.mCustomerMovingState);
        }
    }
}
