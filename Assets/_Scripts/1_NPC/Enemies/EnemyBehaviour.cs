using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Restaurant Restaurant;
    public Transform AttackPoint;

    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashAttack = Animator.StringToHash("Attacking");
    #endregion

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

    private NPCBaseState _currentState;
    public NPCBaseState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    public readonly EnemyMoveState mEnemyMoveState = new EnemyMoveState();
    public readonly EnemyAttackState mEnemyAttackState = new EnemyAttackState();

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgent.stoppingDistance = 1.1f;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navMeshAgent.destination = AttackPoint.position;
        TransitionToState(mEnemyMoveState);
    }

    // Update is called once per frame
    void Update()
    {
        if (NavMeshAgent == null) return;

        _currentState.Update();
    }
    public void TransitionToState(NPCBaseState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        _currentState.EnterState(this);
    }

    public bool HasEnemyReached()
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
