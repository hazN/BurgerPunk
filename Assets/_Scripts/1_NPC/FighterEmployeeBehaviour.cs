using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FighterEmployeeBehaviour : Actor
{
    #region Animator Parameters
    public readonly int m_HashMoving = Animator.StringToHash("Moving");
    public readonly int m_HashAttacking = Animator.StringToHash("Attacking");
    public readonly int m_HashDead = Animator.StringToHash("Dead");
    public readonly int m_HashHurt = Animator.StringToHash("Hurt");
    #endregion
    public float Damage = 20f;
    private bool m_IsDead = false;
    private Actor _enemy;

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

    public readonly FighterEmployeeIdleState mEmployeeIdleState = new FighterEmployeeIdleState();
    public readonly FighterEmployeeMoveState mEmployeeMoveState = new FighterEmployeeMoveState();
    public readonly FighterEmployeeAttackState mEmployeeAttackState = new FighterEmployeeAttackState();
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = 1.1f;
        _animator = GetComponent<Animator>();
        OnDeath += () =>
        {
            m_IsDead = true;
            _navMeshAgent.speed = 0f;
            _animator.SetBool(m_HashDead, true);
            StartCoroutine(Despawn());
        };
        OnHit += () =>
        {
            _animator.SetTrigger(m_HashHurt);
        };
        TransitionToState(mEmployeeIdleState);
    }


    // Update is called once per frame
    void Update()
    {
        if (m_IsDead) return;
        if(_navMeshAgent == null) return;
        if (_enemy)
            _navMeshAgent.destination = _enemy.transform.position;
        _currentState.Update();
    }

    public void TransitionToState(NPCBaseState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        _currentState.EnterState(this);
    }

    public void SendDamage()
    {
        if (_enemy)
        {
            _enemy.TakeDamage(Damage);
            if(!_enemy.IsAlive())
            {
                _enemy = null;
                TransitionToState(mEmployeeIdleState);
            }
        }
        else
            TransitionToState(mEmployeeIdleState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemy != null)
        {
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            _enemy = other.GetComponent<EnemyBehaviour>();
            _navMeshAgent.destination = _enemy.transform.position;
            TransitionToState(mEmployeeMoveState);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(_enemy == null)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemy = other.GetComponent<EnemyBehaviour>();
                _navMeshAgent.destination = _enemy.transform.position;
                TransitionToState(mEmployeeMoveState);
            }
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
