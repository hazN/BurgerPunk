using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CustomerManager : MonoBehaviour
{
    public List<GameObject> CustomersList = new List<GameObject>();
    public List<NPCTarget> TargetList = new List<NPCTarget>();
    public Restaurant mRestaurant;
    //public List<Order> OrdersList = new List<Order>();
    public Transform SpwanPoint;
    public Transform POS_Area;
    public bool IsSomeonePlacingOrder = false;

    [SerializeField]
    private int TotalOccupiedPlaces = 0;
    private List <CustomerBehaviour> _customerBehavioursList = new List<CustomerBehaviour>();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < TargetList.Count; i++)
            CreateCustomer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateCustomer()
    {
        GameObject customer = Instantiate(CustomersList[Random.Range(0, CustomersList.Count)], SpwanPoint);
        CustomerBehaviour customerBehaviour = customer.GetComponent<CustomerBehaviour>();
        customerBehaviour.Manager = this;
        customerBehaviour.Restaurant = mRestaurant;
        customerBehaviour.POS_Area = POS_Area;
        customerBehaviour.Speed = Random.Range(0.2f, 1f);
        customerBehaviour.Wait_Target = TargetList[TotalOccupiedPlaces++];
        _customerBehavioursList.Add(customerBehaviour);
    }
}
