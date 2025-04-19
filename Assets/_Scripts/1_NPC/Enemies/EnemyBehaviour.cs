using BurgerPunk.Movement;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour :  Actor
{
    public float AttackPoints = 5f;
    public Transform TargetPoint;
    public EnemySpawnManager SpawnerManger;

    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashAttack = Animator.StringToHash("Attacking");
    public readonly int m_HashDead = Animator.StringToHash("Dead");
    public readonly int m_HashHurt = Animator.StringToHash("Hurt");
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

    private GameObject m_PlayerObject;
    private bool m_IsTargetPlayer = false;
    private bool m_IsDead = false;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgent.stoppingDistance = 0.8f;
        OnDeath += () => {
            m_IsDead = true;
            _navMeshAgent.speed = 0f;
            _animator.SetBool(m_HashDead, true);
        };
        OnHit += () =>
        {
            _animator.SetTrigger(m_HashHurt);
        };
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navMeshAgent.destination = TargetPoint.position;
        TransitionToState(mEnemyMoveState);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsDead) return;
        if (NavMeshAgent == null) return;

        if(m_IsTargetPlayer)
            _navMeshAgent.destination = m_PlayerObject.transform.position;
        _currentState.Update();
    }

    public void TransitionToState(NPCBaseState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        _currentState.EnterState(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_IsTargetPlayer = true;
            m_PlayerObject = other.gameObject;
            _navMeshAgent.destination = m_PlayerObject.transform.position;
            TransitionToState(mEnemyMoveState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_IsTargetPlayer = false;
            _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
            TransitionToState(mEnemyMoveState);
        }
    }

    public void SendDamage()
    {
        if (m_IsTargetPlayer)
        { 
            if (m_PlayerObject != null)
            {
                Debug.Log("Attacking Player: -" + AttackPoints);
                m_PlayerObject.GetComponent<FirstPersonController>().TakeDamage(AttackPoints);
                return;
            }
        }
        else
        {
            Restaurant.Instance.TakeDamage(AttackPoints);
        }
    }

}
