using UnityEngine;

public class CustomerMovingState : CustomerBaseState
{
    private Vector3 _targetPosition = Vector3.zero;
    public override void EnterState(CustomerBehaviour customer)
    {
        Debug.Log("Entering moving state");
        customer.Animator.SetBool(customer.m_HashMove, true);
        _targetPosition = customer.NPCTarget_First.transform.position;
    }

    public override void Update(CustomerBehaviour customer)
    {

        if (Vector3.Distance(customer.transform.position, _targetPosition) <= customer.DistanceMargin)
        {
            customer.TransitionToState(customer.mCustomerOrderingState);
        }
    }
}
