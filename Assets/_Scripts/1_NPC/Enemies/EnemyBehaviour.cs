using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Restaurant Restaurant;

    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashOrder1 = Animator.StringToHash("Attacking");
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
