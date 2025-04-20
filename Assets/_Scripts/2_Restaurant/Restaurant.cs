using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct OrderItem
{
    public FoodTypes Type;
    public string Name;
    public float Cost;
}

[System.Serializable]
public struct RestuarantEquipmentWrapper
{
    public RestaurantEquipmentData RestaurantEquipment;
    public GameObject WorldEntity;
}

public struct PendingOrder
{
    public CustomerBehaviour Customer;
    public EmployeeBehaviour Employee;
    public List<OrderItem> OrderItemsList;
    public float TotalCost;
    public List<Transform> MachinesList;

    public PendingOrder(PendingOrder pendingOrder)
    {
        Customer = pendingOrder.Customer;
        Employee = pendingOrder.Employee;
        OrderItemsList = pendingOrder.OrderItemsList;
        TotalCost = pendingOrder.TotalCost;
        MachinesList = new List<Transform>();
        MachinesList.AddRange(pendingOrder.MachinesList);
    }
}

public class Restaurant : MonoBehaviour
{
    public static Restaurant Instance { get; private set; } = null;
    public float HealthPoints = 1000f;

    [Header("Orders")]
    public List<RestuarantEquipmentWrapper> EquipmentsList = new List<RestuarantEquipmentWrapper>();
    public List<PendingOrder> PendingOrdersList = new List<PendingOrder>();
    public List<PendingOrder> ReadyOrderList = new List<PendingOrder>();
    public float TotalSale = 0f;

    [Header("Employees")]
    public List<EmployeeBehaviour> EmployeesList = new List<EmployeeBehaviour>();
    public List<GameObject> EmployeesPrefabsList = new List<GameObject>();
    public Transform EmployeeSpawnTile;
    public Transform OrderRack;

    private void Awake()
    {
        if (Instance != null)
            throw new UnityException("There can only be one Restaurant object at a time");
        Instance = this;
        ReadyOrderList.Capacity = 4;
    }

    private void Start()
    {
        SpawnEmployees();
    }

    /// <summary>
    /// Spawn given "count" number of employees
    /// </summary>
    /// <param name="count">No. of employees to be spawned</param>
    public void SpawnEmployees(int count = 1)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject employeeObject = Instantiate(EmployeesPrefabsList[Random.Range(0, EmployeesPrefabsList.Count)], EmployeeSpawnTile);
            EmployeeBehaviour employeeBehaviour = employeeObject.GetComponent<EmployeeBehaviour>();
            employeeBehaviour.Orders_Rack = OrderRack;
            employeeBehaviour.POS_Area = CustomerManager.Instance.POS_Area;
            EmployeesList.Add(employeeBehaviour);
        }
    }


    /// <summary>
    /// Order given to Customer
    /// </summary>
    /// <param name="customer">Customer who ordered</param>
    public void OrderFulfilled(CustomerBehaviour customer)
    {
        foreach (var order in ReadyOrderList)
        {
            if (order.Customer == customer)
            {
                ReadyOrderList.Remove(order);
                return;
            }
        }
    }

    /// <summary>
    /// Order is ready to be delivered
    /// </summary>
    /// <param name="employee">Employee who prepared order</param>
    public void OrderWrapUp(EmployeeBehaviour employee)
    {
        employee.IsBusy = false;
        employee.OrderStacked = false;
        employee.OrderItemsMade = 0;
        ReadyOrderList.Add(employee.PendingOrder);
        AssignTask(employee);
    }

    /// <summary>
    /// Tell Employee to do stuff
    /// </summary>
    /// <param name="employee">Employee who will prepare the order</param>
    public void AssignTask(EmployeeBehaviour employee = null)
    {
        if (ReadyOrderList.Count == 4)
            return;
        if (!PendingOrdersList.Any())
            return;
        if (employee == null)
        {
            foreach (var freeEmployee in EmployeesList)
            {
                if (!freeEmployee.IsBusy)
                {
                    employee = freeEmployee;
                    break;
                }
            }
        }

        if (employee == null)
        {
            Debug.Log("All Employees are busy");
            return;
        }

        employee.PendingOrder = new PendingOrder(PendingOrdersList[0]);
        employee.IsBusy = true;
        employee.NavMeshAgent.destination = employee.PendingOrder.MachinesList[0].position;
        employee.Animator.SetBool(employee.m_HashMove, true);
        PendingOrdersList.RemoveAt(0);
    }

    public void GetRandomOrder(CustomerBehaviour customer)
    {
        string orderStr = string.Empty;
        int orderSize = EquipmentsList.Count <= 1 ? 1 : Random.Range(1, EquipmentsList.Count + 1);
        int itemOrdered = 0;
        PendingOrder pendingOrder = new PendingOrder();
        pendingOrder.MachinesList = new List<Transform>();
        pendingOrder.TotalCost = 0f;
        pendingOrder.Customer = null;
        for (int i = 0; i < EquipmentsList.Count; i++)
        {
            var item = EquipmentsList[i];
            if (item.RestaurantEquipment.IsEquipped)
            {
                itemOrdered++;
                OrderItem orderItem = item.RestaurantEquipment.OrderItemsList[Random.Range(0, item.RestaurantEquipment.OrderItemsList.Count)];
                pendingOrder.OrderItemsList.Add(orderItem);
                pendingOrder.MachinesList.Add(item.WorldEntity.transform);
                pendingOrder.TotalCost += orderItem.Cost;
            }
            if (itemOrdered == orderSize)
                break;
        }
        pendingOrder.Customer = customer;
        PendingOrdersList.Add(pendingOrder);

        AssignTask();

        customer.OrderStr = orderStr;
        customer.Animator.SetTrigger(customer.m_HashOrder1);
    }

    public void TakeDamage(float hp)
    {
        HealthPoints -= hp;
    }
}


