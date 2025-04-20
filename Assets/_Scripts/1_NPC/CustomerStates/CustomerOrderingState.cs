using UnityEngine;

public class CustomerOrderingState : NPCBaseState
{
    CustomerBehaviour customer;
    public override void EnterState<T>(T npc)
    {
        customer = npc as CustomerBehaviour;
        CustomerManager.Instance.IsSomeonePlacingOrder = true;
        Restaurant.Instance.GetRandomOrder(customer);
    }

    public override void Update()
    {

    }
}
