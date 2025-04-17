using UnityEngine;

public class CustomerOrderingState : NPCBaseState
{
    public override void EnterState<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
        customer.Manager.IsSomeonePlacingOrder = true;
        customer.Restaurant.GetRandomOrder(customer);
    }

    public override void Update<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
    }
}
