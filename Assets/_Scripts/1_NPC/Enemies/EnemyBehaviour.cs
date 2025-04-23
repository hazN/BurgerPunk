using BurgerPunk.Movement;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : Actor
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

    public FirstPersonController mPlayerObject;
    public FighterEmployeeBehaviour mFighterEmployeeObject;
    public bool mIsTargetPlayer = false;
    public bool mIsTargetEmployee = false;
    private bool m_IsDead = false;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgent.stoppingDistance = 1.1f;
        OnDeath += () =>
        {
            m_IsDead = true;
            _navMeshAgent.speed = 0f;
            _animator.SetBool(m_HashDead, true);
            StartCoroutine(Despawn());
            GameManager.Instance.EnemyDied();
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

        if (mIsTargetPlayer)
            _navMeshAgent.destination = mPlayerObject.transform.position;
        else if (mIsTargetEmployee)
        {
            if (mFighterEmployeeObject != null)
            {
                _navMeshAgent.destination = mFighterEmployeeObject.transform.position;
            }
            else
            {
                mIsTargetEmployee = false;
            }
        }
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
        if (other.CompareTag("Player"))
        {
            mIsTargetPlayer = true;
            mIsTargetEmployee = false;
            if (mPlayerObject == null)
                mPlayerObject = other.GetComponent<FirstPersonController>();
            TransitionToState(mEnemyMoveState);
        }
        else if (other.CompareTag("Employee"))
        {
            mIsTargetPlayer = false;
            mIsTargetEmployee = true;
            if (mFighterEmployeeObject == null)
                mFighterEmployeeObject = other.GetComponent<FighterEmployeeBehaviour>();
            _navMeshAgent.destination = mFighterEmployeeObject.transform.position;
            TransitionToState(mEnemyMoveState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mIsTargetPlayer = false;
            _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
            TransitionToState(mEnemyMoveState);
        }
        else if (other.CompareTag("Employee"))
        {
            mFighterEmployeeObject = null;
            _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
            TransitionToState(mEnemyMoveState);
        }
    }

    public void SendDamage()
    {
        if (mIsTargetPlayer)
        {
            if (mPlayerObject != null)
            {
                Debug.Log("Attacking Player: -" + AttackPoints);
                mPlayerObject.TakeDamage(AttackPoints);
                return;
            }
        }
        else if (mIsTargetEmployee)
        {
            if(mFighterEmployeeObject != null)
            {
                Debug.Log("Attacking Employee: -" + AttackPoints);
                mFighterEmployeeObject.TakeDamage(AttackPoints);
                if(!mFighterEmployeeObject.IsAlive())
                {
                    mIsTargetEmployee = false;
                    mFighterEmployeeObject = null;
                    _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
                    TransitionToState(mEnemyMoveState);
                }
                return;
            }
        }
        else
        {
            Restaurant.Instance.TakeDamage(AttackPoints);
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
