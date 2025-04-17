using UnityEngine;
using UnityEngine.AI;

public class CustomerMovingState : NPCBaseState
{
    private Vector3 _targetPosition = Vector3.zero;
    public override void EnterState<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
        customer.NavMeshAgent.speed = customer.Speed;
        customer.NavMeshAgent.stoppingDistance = customer.DistanceMargin;
        customer.Animator.SetBool(customer.m_HashMove, true);
        if(customer.IsOrderPlaced)
            _targetPosition = customer.Wait_Target.transform.position;
        else 
            _targetPosition = customer.POS_Area.position;

        customer.NavMeshAgent.destination = _targetPosition;
    }

    public override void Update<T>(T npc)
    {
        CustomerBehaviour customer = npc as CustomerBehaviour;
        if (!customer.IsOrderPlaced && customer.Manager.IsSomeonePlacingOrder)
        {
            customer.Animator.SetBool(customer.m_HashMove, false);
            customer.TransitionToState(customer.mCustomerWaitingState);
            customer.NavMeshAgent.speed = 0f;
            return;
        }

        if (!customer.NavMeshAgent.pathPending)
        {
            if (customer.NavMeshAgent.remainingDistance <= customer.NavMeshAgent.stoppingDistance)
            {
                if (!customer.NavMeshAgent.hasPath || customer.NavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    customer.Animator.SetBool(customer.m_HashMove, false);
                    if (customer.IsOrderPlaced)
                    {
                        if (customer.Wait_Target.TargetType == TargetType.TakeOut)
                            customer.TransitionToState(customer.mCustomerWaitingState);
                        else
                            customer.TransitionToState(customer.mCustomerSittigState);
                    }
                    else customer.TransitionToState(customer.mCustomerOrderingState);
                }
            }
        }
    }
}
