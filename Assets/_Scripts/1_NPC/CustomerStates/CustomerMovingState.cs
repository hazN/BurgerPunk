using UnityEngine;
using UnityEngine.AI;

public class CustomerMovingState : CustomerBaseState
{
    private Vector3 _targetPosition = Vector3.zero;
    public override void EnterState(CustomerBehaviour customer)
    {
        customer.NavMeshAgent.speed = customer.Speed;
        customer.NavMeshAgent.stoppingDistance = customer.DistanceMargin;
        customer.Animator.SetBool(customer.m_HashMove, true);
        if(customer.IsOrderPlaced)
            _targetPosition = customer.Wait_Target.transform.position;
        else 
            _targetPosition = customer.POS_Area.position;

        customer.NavMeshAgent.destination = _targetPosition;
    }

    public override void Update(CustomerBehaviour customer)
    {
        if (!customer.IsOrderPlaced && customer.Manager.IsSomeonePlacingOrder)
        {
            customer.Animator.SetBool(customer.m_HashMove, false);
            customer.TransitionToState(customer.mCustomerWaitingState);
            customer.NavMeshAgent.speed = 0f;
            return;
        }

        if (Vector3.Distance(customer.transform.position, _targetPosition) <= customer.DistanceMargin)
        {
            customer.NavMeshAgent.speed = 0f;
            customer.Animator.SetBool(customer.m_HashMove, false);
            if (customer.IsOrderPlaced)
            {
                customer.Manager.SetOccupied(customer);
                if (customer.Wait_Target.TargetType == TargetType.WaitingQueue)
                    customer.TransitionToState(customer.mCustomerWaitingState);
                else 
                    customer.TransitionToState(customer.mCustomerSittigState);
            }
            else customer.TransitionToState(customer.mCustomerOrderingState);
        }
    }
}
