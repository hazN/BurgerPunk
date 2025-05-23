using UnityEngine;
using UnityEngine.AI;

public class CustomerMovingState : NPCBaseState
{
    CustomerBehaviour customer;
    private Vector3 _targetPosition = Vector3.zero;
    public override void EnterState<T>(T npc)
    {
        customer = npc as CustomerBehaviour;
        customer.NavMeshAgent.speed = customer.Speed;
        customer.NavMeshAgent.stoppingDistance = customer.DistanceMargin;
        customer.Animator.SetBool(customer.m_HashMove, true);
        if (customer.IsOrderFulfilled)
        {
            _targetPosition = CustomerManager.Instance.SpawnPoint.position;
        }
        else if (customer.IsOrderPlaced)
            _targetPosition = customer.Wait_Target.transform.position;
        else 
            _targetPosition = customer.OrderTile.position;

        customer.NavMeshAgent.destination = _targetPosition;
    }

    public override void Update()
    {
        if (!customer.IsOrderPlaced && CustomerManager.Instance.IsSomeonePlacingOrder)
        {
            customer.Animator.SetBool(customer.m_HashMove, false);
            customer.TransitionToState(customer.mCustomerWaitingState);
            customer.NavMeshAgent.speed = 0f;
            return;
        }

        if(Helper.HaveReached(customer.NavMeshAgent))
        {
            customer.Animator.SetBool(customer.m_HashMove, false);
            if(customer.IsOrderFulfilled)
            {
                CustomerManager.Instance.DespawnCustomer(customer);
                return;
            }
            if (customer.IsOrderPlaced)
            {
                if (customer.Wait_Target.TargetType == TargetType.TakeOut)
                    customer.TransitionToState(customer.mCustomerWaitingState);
                else
                    customer.TransitionToState(customer.mCustomerSittigState);
            }
            else
            {
                //customer.transform.forward = CustomerManager.Instance.OrderTile.transform.forward;
                customer.TransitionToState(customer.mCustomerOrderingState); 
            }
        }
    }
}
