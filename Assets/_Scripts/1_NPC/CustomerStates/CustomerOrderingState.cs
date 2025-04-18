using UnityEngine;

public class CustomerOrderingState : NPCBaseState
{
    CustomerBehaviour customer;
    public override void EnterState<T>(T npc)
    {
        customer = npc as CustomerBehaviour;
        customer.Manager.IsSomeonePlacingOrder = true;
        customer.Restaurant.GetRandomOrder(customer);
    }

    public override void Update()
    {

    }
}
