using UnityEngine;
using UnityEngine.AI;

public class FighterEmployeeBehaviour : Actor
{
    #region Animator Parameters
    public readonly int m_HashMoving = Animator.StringToHash("Moving");
    public readonly int m_HashAttacking = Animator.StringToHash("Attacking");
    #endregion

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
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = 0.8f;
        _animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (_enemy == null)
        {
            return;
        }
        if (Helper.HaveReached(_navMeshAgent))
        {
            _animator.SetBool(m_HashAttacking, true);
            _animator.SetBool(m_HashMoving, false);
        }
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
            _animator.SetBool(m_HashMoving, true);
        }
    }
}
