using BurgerPunk.Player;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehaviour : Interactable
{
    #region Animator Parameters
    public readonly int m_HashMove = Animator.StringToHash("Moving");
    public readonly int m_HashOrder1 = Animator.StringToHash("Order1");
    public readonly int m_HashSit = Animator.StringToHash("Sit");
    public readonly int m_HashEat = Animator.StringToHash("Eat");
    #endregion
    
    public float Speed = 1f;
    public float DistanceMargin = 0.8f;

    public bool IsOrderPlaced = false;
    public bool IsOrderFulfilled = false;
    public NPCTarget Wait_Target;
    public Transform OrderTile;
    public GameObject Stool;
    public GameObject SpotLight;
    [SerializeField] public InteractableCone InteractCone;

    [HideInInspector]
    public List<FoodTypes> FoodTypesList = new List<FoodTypes>();

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

    public readonly CustomerMovingState mCustomerMovingState = new CustomerMovingState();
    public readonly CustomerOrderingState mCustomerOrderingState = new CustomerOrderingState();
    public readonly CustomerSittingState mCustomerSittigState = new CustomerSittingState();
    public readonly CustomerWaitingState mCustomerWaitingState = new CustomerWaitingState();
    private PlayerRestaurant playerRestaurant;
    private System.Action interactionHandler;
    private void Awake()
    {
        playerRestaurant = FindFirstObjectByType<PlayerRestaurant>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        interactionHandler += () =>
        {
            if (playerRestaurant.IsOrderComplete())
            {
                PendingOrder order = playerRestaurant.GetCurrentOrder();
                if (order.Customer == this)
                {
                    if (SpotLight != null)
                        SpotLight.SetActive(false);
                    InteractCone.gameObject.SetActive(false);

                    _animator.SetTrigger(m_HashEat);

                    playerRestaurant.ClearOrder();
                    // Add customer audio here
                    AudioManager.Instance.PlayRandomCustomerBark();
                    AudioManager.Instance.customerServed.Play();

                    GameManager.Instance.AddMoney((int)order.TotalCost);

                    GameManager.Instance.customersServedThisDay++;
                    GameManager.Instance.totalCustomersServed++;
                }
                else
                {
                    //Debug.Log("This is not your order!");
                    AudioManager.Instance.wrongCustomer.Play();
                }
            }
        };

        OnInteracted += interactionHandler;
    }
    private void OnDestroy()
    {
        if (interactionHandler != null)
        {
            OnInteracted -= interactionHandler;
        }
    }

    void Start()
    {
        TransitionToState(mCustomerMovingState);
    }


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
}
