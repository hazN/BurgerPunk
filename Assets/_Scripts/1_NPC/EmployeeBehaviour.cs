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

        if (Helper.HaveReached(_navMeshAgent) && IsBusy)
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
}
