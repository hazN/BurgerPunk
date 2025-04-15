using UnityEngine;
using UnityEngine.AI;

public class CustomerBehaviour : MonoBehaviour
{
    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Move");
    public readonly int m_HashOrder1 = Animator.StringToHash("Order1");
    #endregion
    
    private Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
    }

    public readonly float DistanceMargin = 0.5f;

    public NPCTarget NPCTarget_First;
    public NPCTarget NPCTarget_Second;

    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent;
        set => _navMeshAgent = value;
    }

    private CustomerBaseState _currentState;
    public CustomerBaseState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    public readonly CustomerMovingState mCustomerMovingState = new CustomerMovingState();
    public readonly CustomerOrderingState mCustomerOrderingState = new CustomerOrderingState();
    public readonly CustomerSittingState mCustomerSittigState = new CustomerSittingState();
    public readonly CustomerWaitingState mCustomerWaitingState = new CustomerWaitingState();

    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.destination = NPCTarget_First.transform.position;
        TransitionToState(mCustomerMovingState);
    }


    void Update()
    {
        if (NavMeshAgent == null) return;

        _currentState.Update(this);

        // Check if agent is at the destination

        // If destination is chair, sit down

        // If destination is cash counter, make an order

        // If destination is pickup line, wait for order
    }

    public void TransitionToState(CustomerBaseState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        _currentState.EnterState(this);
    }
}
