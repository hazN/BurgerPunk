using UnityEngine;

public class EatAndLeave : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CustomerBehaviour customer = animator.gameObject.GetComponent<CustomerBehaviour>();
        if (customer != null)
        {
            if(customer.IsOrderFulfilled)
            {
                customer.TransitionToState(customer.mCustomerMovingState);
            }
            else
            {
                customer.FoodTypesList.Clear();
                customer.IsOrderFulfilled = true;
                customer.Animator.SetBool(customer.m_HashSit, false);
                customer.Stool.gameObject.SetActive(false);
            }
        }
    }
}
