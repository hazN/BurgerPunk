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
    public Transform SpwanPoint;
    public Transform POS_Area;
    public bool IsSomeonePlacingOrder = false;

    [SerializeField]
    private int TotalOccupiedPlaces = 0;
    private List <CustomerBehaviour> _customerBehavioursList = new List<CustomerBehaviour>();

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
        if(timeInSeconds == 0f)
            for (int i = 0; i < count; i++)
                CreateCustomer();
        else
        {
            StartCoroutine(SpawnCustomersDelayed(count, timeInSeconds));
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

    private void CreateCustomer()
    {
        GameObject customer = Instantiate(CustomersList[Random.Range(0, CustomersList.Count)], SpwanPoint);
        CustomerBehaviour customerBehaviour = customer.GetComponent<CustomerBehaviour>();
        customerBehaviour.POS_Area = POS_Area;
        customerBehaviour.Speed = Random.Range(0.2f, 1f);
        customerBehaviour.Wait_Target = TargetList[TotalOccupiedPlaces++];
        _customerBehavioursList.Add(customerBehaviour);
    }
}
