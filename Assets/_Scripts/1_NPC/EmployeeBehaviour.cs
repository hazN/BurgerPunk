using UnityEngine;
using UnityEngine.AI;

public class EmployeeBehaviour : MonoBehaviour
{
    public PendingOrder PendingOrder;
    public bool IsBusy = false;

    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashCooking = Animator.StringToHash("Cooking");
    public readonly int m_HashStackOrder = Animator.StringToHash("StackOrder");
    #endregion

    public Transform POS_Area;
    public Transform Orders_Rack;

    public CustomerManager CustomerManager;
    public Restaurant Restaurant;

    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent;
        set => _navMeshAgent = value;
    }

    private Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
        private set => _animator = value;
    }

    public int OrderItemsMade = 0;
    public bool OrderStacked = false;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = 1.1f;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (NavMeshAgent == null) return;

        if (HasEmployeeReached() && IsBusy)
        {
            _animator.SetBool(m_HashMove, false);
            if(OrderStacked)
            {
                _animator.SetTrigger(m_HashStackOrder);
            }
            else {
                _animator.SetBool(m_HashCooking, true);
            }
        }
    }

    private bool HasEmployeeReached()
    {
        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
