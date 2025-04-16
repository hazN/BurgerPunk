using UnityEngine;
using UnityEngine.AI;

public class CustomerBehaviour : MonoBehaviour
{
    public CustomerManager Manager;
    public Restaurant Restaurant;

    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashOrder1 = Animator.StringToHash("Order1");
    #endregion
    
    private Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
    }
    public float Speed = 1f;
    public float DistanceMargin = 1.1f;

    public bool IsOrderPlaced = false;
    public NPCTarget Wait_Target;
    public Transform POS_Area;

    [HideInInspector]
    public string OrderStr = string.Empty;

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

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        TransitionToState(mCustomerMovingState);
    }


    void Update()
    {
        if (NavMeshAgent == null) return;

        _currentState.Update(this);
    }

    public void TransitionToState(CustomerBaseState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        _currentState.EnterState(this);
    }
}
