using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; } = null;
    public List<GameObject> CustomersList = new List<GameObject>();
    public List<NPCTarget> TargetList = new List<NPCTarget>();
    //public List<Order> OrdersList = new List<Order>();
    public Transform SpawnPoint;
    public Transform OrderTile;
    public bool IsSomeonePlacingOrder = false;

    [SerializeField]
    private int TotalOccupiedPlaces = 0;
    private List <CustomerBehaviour> _customerBehavioursList = new List<CustomerBehaviour>();
    //public bool DayStarted = false;
    //public bool DayFinished = false;

    private void Awake()
    {
        if (Instance != null)
            throw new UnityException("There can only be one Customer Manager at a time");
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCustomers(int count = 1, float timeInSeconds = 0f)
    {
        Debug.Log("Begin spawn customers");
        if(timeInSeconds == 0f)
            for (int i = 0; i < count; i++)
                CreateCustomer();
        else
        {
            StartCoroutine(SpawnCustomersDelayed(count, timeInSeconds));
        }
    }

    public void DespawnCustomer(CustomerBehaviour customer)
    {
        if (customer != null)
        {
            customer.Wait_Target.IsOccupied = false;
            _customerBehavioursList.Remove(customer);
            Destroy(customer.gameObject);
            if(!GameManager.Instance.dayActivitiesComplete)
                SpawnCustomers(1, 5f);
        }
    }

    private IEnumerator SpawnCustomersDelayed(int count,float time)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(time);
            CreateCustomer();
        }
    }

    private int GetFreeSpot()
    {
        int freeSpot = -1;

        for(int i = 0; i < TargetList.Count; i++)
        {
            if (!TargetList[i].IsOccupied)
                return i;
        }
        
        return freeSpot;
    }

    private void CreateCustomer()
    {
        
        int freeSpot = GetFreeSpot();

        Debug.Log("acquiring free spot: " + freeSpot);
        if (freeSpot == -1)
            return;

        if (GameManager.Instance.dayActivitiesComplete)
        {
            Debug.Log("Try create customer failed, day is complete");
        }

        Debug.Log("Customer created");
        GameObject customer = Instantiate(CustomersList[Random.Range(0, CustomersList.Count)], SpawnPoint);
        CustomerBehaviour customerBehaviour = customer.GetComponent<CustomerBehaviour>();
        customerBehaviour.OrderTile = OrderTile;
        customerBehaviour.Speed = Random.Range(0.2f, 1f);
        customerBehaviour.Wait_Target = TargetList[freeSpot];
        customerBehaviour.Wait_Target.IsOccupied = true;
        _customerBehavioursList.Add(customerBehaviour);
    }

    public void ClearCustomers()
    {
        foreach (var customer in _customerBehavioursList)
        {
            if (customer != null)
                Destroy(customer.gameObject);
        }
        _customerBehavioursList.Clear();
    }
}
