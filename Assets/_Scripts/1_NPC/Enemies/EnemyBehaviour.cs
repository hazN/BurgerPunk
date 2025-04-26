using BurgerPunk.Movement;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : Actor
{
    public float AttackPoints = 5f;
    public Transform TargetPoint;
    public EnemySpawnManager SpawnerManger;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private HealthBar healthBar;

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
    public PlaceableObject mPlaceableObject;
    public bool mIsTargetPlayer = false;
    public bool mIsTargetEmployee = false;
    public bool mIsTargetPlaceable = false;
    private bool m_IsDead = false;

    float attackVoicelineTimer = 0.0f;
    float attackVoicelineTime = 5.0f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgent.stoppingDistance = 1.5f;
        OnDeath += () =>
        {
            m_IsDead = true;
            _navMeshAgent.speed = 0f;
            _animator.SetBool(m_HashDead, true);
            StartCoroutine(Despawn());
            GameManager.Instance.EnemyDied();
            audioSource.clip = AudioManager.Instance.GetEnemyDeathClip();
            audioSource.Play();
            GameManager.Instance.AddMoney(50);
        };
        OnHit += () =>
        {
            _animator.SetTrigger(m_HashHurt);
            audioSource.clip = AudioManager.Instance.GetEnemyDamagedClip();
            audioSource.Play();
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

        attackVoicelineTimer += Time.deltaTime;
        if (attackVoicelineTimer > attackVoicelineTime)
        {
            attackVoicelineTime = Random.Range(4.0f, 10.0f);
            attackVoicelineTimer = 0;

            audioSource.clip = AudioManager.Instance.GetEnemyAttackClip();
            audioSource.Play();
        }

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
                _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
                TransitionToState(mEnemyMoveState);
            }
        }
        else if(mIsTargetPlaceable)
        {
            if (mPlaceableObject != null)
            {
                _navMeshAgent.destination = mPlaceableObject.transform.position;
            }
            else
            {
                mIsTargetPlaceable = false;
                _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
                TransitionToState(mEnemyMoveState);
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
            mIsTargetEmployee = false;
            mIsTargetPlaceable = false;
            mIsTargetPlayer = true;
            if (mPlayerObject == null)
                mPlayerObject = other.GetComponent<FirstPersonController>();
            TransitionToState(mEnemyMoveState);
        }
        else if (other.CompareTag("Employee"))
        {
            mIsTargetPlayer = false;
            mIsTargetPlaceable = false;
            mIsTargetEmployee = true;
            if (mFighterEmployeeObject == null)
                mFighterEmployeeObject = other.GetComponent<FighterEmployeeBehaviour>();
            _navMeshAgent.destination = mFighterEmployeeObject.transform.position;
            TransitionToState(mEnemyMoveState);
        }
        else if (other.CompareTag("Placeable"))
        {
            mIsTargetPlayer = false;
            mIsTargetEmployee = false;
            mIsTargetPlaceable = true;
            if (mPlaceableObject == null)
                mPlaceableObject = other.GetComponent<PlaceableObject>();
            _navMeshAgent.destination = mPlaceableObject.transform.position;
            TransitionToState(mEnemyMoveState);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!mIsTargetPlayer && !mIsTargetEmployee && !mIsTargetPlaceable)
        {
            OnTriggerEnter(other);
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
            mIsTargetEmployee = false;
            mFighterEmployeeObject = null;
            _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
            TransitionToState(mEnemyMoveState);
        }
        else if(other.CompareTag("Placeable"))
        {
            mIsTargetPlaceable = false;
            mPlaceableObject = null;
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
                mPlayerObject.TakeDamage(AttackPoints);
                return;
            }
        }
        else if (mIsTargetEmployee)
        {
            if(mFighterEmployeeObject != null)
            {
                if (!mFighterEmployeeObject.IsAlive())
                {
                    mIsTargetEmployee = false;
                    mFighterEmployeeObject = null;
                    _navMeshAgent.destination = SpawnerManger.targetPoint[Random.Range(0, SpawnerManger.targetPoint.Length)].position;
                    TransitionToState(mEnemyMoveState);
                }
                else mFighterEmployeeObject.TakeDamage(AttackPoints);
                
                return;
            }
        }
        else if (mIsTargetPlaceable)
        {
            if( mPlaceableObject != null)
            {
                mPlaceableObject.TakeDamage(AttackPoints);
                if(!mPlaceableObject.IsAlive())
                {
                    mIsTargetPlaceable = false;
                    mPlaceableObject = null;
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
        Destroy(healthBar);
    }
}
