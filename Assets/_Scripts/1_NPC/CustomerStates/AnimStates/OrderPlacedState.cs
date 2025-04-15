using UnityEngine;

public class OrderPlacedState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CustomerBehaviour customer = animator.gameObject.GetComponent<CustomerBehaviour>();
        if(customer != null )
        {
            customer.IsOrderPlaced = true;
        }
    }
}
