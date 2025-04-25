using BurgerPunk;
using BurgerPunk.Player;
using BurgerPunk.UI;
using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
    public Transform[] TargetList;
    public GameObject Prefab;
}

public class PendingOrder
{
    private static int _orderIdCounter = 0;
    public int OrderId;
    public CustomerBehaviour Customer;
    public EmployeeBehaviour Employee;
    public List<OrderItem> OrderItemsList;
    public float TotalCost;
    public List<Transform> MachinesList;
    public int TrayId;

    public PendingOrder(PendingOrder pendingOrder)
    {
        Customer = pendingOrder.Customer;
        Employee = pendingOrder.Employee;
        OrderItemsList = pendingOrder.OrderItemsList;
        TotalCost = pendingOrder.TotalCost;
        MachinesList = new List<Transform>();
        MachinesList.AddRange(pendingOrder.MachinesList);
        TrayId = pendingOrder.TrayId;
        if (OrderId == 0)
            OrderId = _orderIdCounter++;
        else
            OrderId = pendingOrder.OrderId;
    }
    public PendingOrder()
    {
        OrderId = _orderIdCounter++;
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
    public PendingOrder PlayerOrder;
    public float TotalSale = 0f;

    [Header("Employees")]
    public List<EmployeeBehaviour> EmployeesList = new List<EmployeeBehaviour>();

    public List<GameObject> EmployeesPrefabsList = new List<GameObject>();
    public Transform[] EmployeeSpawnTile;
    public List<Transform> OrderTraysList;
    private int[] _assignedTray = new int[4] { 0, 0, 0, 0 };

    public UnityEvent OnRefreshUI;
    public UnityEvent OnDamageTaken;

    [SerializeField] public List<Tray> trays = new List<Tray>();

    private void Awake()
    {
        if (Instance != null)
            throw new UnityException("There can only be one Restaurant object at a time");
        Instance = this;
    }

    private void Start()
    {
        //SpawnEmployees();
    }

    /// <summary>
    /// Spawn given "count" number of employees
    /// </summary>
    /// <param name="count">No. of employees to be spawned</param>
    public void SpawnEmployees(int index  = -1)
    {
        if (index == -1)
            index = Random.Range(0, EmployeesPrefabsList.Count);
        if(index > EmployeesPrefabsList.Count)
        {
            throw new UnityException("Employee Prefab is not present");
        }

        GameObject employeeObject = Instantiate(EmployeesPrefabsList[index], EmployeeSpawnTile[index]);
        EmployeeBehaviour employeeBehaviour = employeeObject.GetComponent<EmployeeBehaviour>();
        employeeBehaviour.POS_Area = CustomerManager.Instance.OrderTile;
        EmployeesList.Add(employeeBehaviour);
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
                Debug.Log("Order Fullfilled");
                //GameManager.Instance.AddToBalance(order.TotalCost);
                ReadyOrderList.Remove(order);
                FreeATray(order.TrayId);
                AssignTask();
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
        trays[employee.PendingOrder.TrayId].AssignOrder(employee.PendingOrder);
        trays[employee.PendingOrder.TrayId].GetComponent<InteractableTray>().enabled = true;
        employee.NavMeshAgent.enabled = false;
        employee.Animator.SetBool(employee.m_HashMove, false);
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
        if (!PendingOrdersList.Any())
            return;
        int tray = GetFreeTray();
        if (ReadyOrderList.Count == 4 || tray == -1)
        {
            Debug.Log("All trays are occupied");
            return;
        }
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
        employee.PendingOrder.TrayId = tray;
        employee.Orders_Rack = OrderTraysList[tray];
        OccupyATray(employee.PendingOrder.TrayId);
        employee.IsBusy = true;
        employee.NavMeshAgent.enabled = true;
        employee.NavMeshAgent.destination = employee.PendingOrder.MachinesList[0].position;
        employee.NavMeshAgent.speed = employee.EmployeeSpeed;
        employee.Animator.SetBool(employee.m_HashMove, true);
        PendingOrdersList.RemoveAt(0);

        OnRefreshUI?.Invoke();
    }

    private int GetFreeTray()
    {
        for (int i = 0; i < _assignedTray.Length; i++)
        {
            if (_assignedTray[i] == 0)
                return i;
        }
        return -1;
    }

    private void FreeATray(int i)
    {
        _assignedTray[i] = 0;
    }

    private void OccupyATray(int i)
    {
        Debug.Log("Tray no. " + i + " occupied");
        _assignedTray[i] = 1;
    }

    public void GetRandomOrder(CustomerBehaviour customer)
    {
        string orderStr = string.Empty;
        int orderSize = EquipmentsList.Count <= 1 ? 1 : Random.Range(1, EquipmentsList.Count + 1);
        int itemOrdered = 0;
        PendingOrder pendingOrder = new PendingOrder();
        pendingOrder.MachinesList = new List<Transform>();
        pendingOrder.OrderItemsList = new List<OrderItem>();
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
                pendingOrder.MachinesList.Add(item.TargetList[Random.Range(0, item.TargetList.Length)]);
                pendingOrder.TotalCost += orderItem.Cost;
                customer.FoodTypesList.Add(orderItem.Type);
            }
            if (itemOrdered == orderSize)
                break;
        }
        pendingOrder.Customer = customer;
        PendingOrdersList.Add(pendingOrder);
        OnRefreshUI?.Invoke();

        AssignTask();
        customer.Animator.SetTrigger(customer.m_HashOrder1);
    }

    public void TakeDamage(float hp)
    {
        HealthPoints -= hp;
        OnDamageTaken?.Invoke();

        if (HealthPoints <= 0)
        {
            FindFirstObjectByType<GameOverUI>().GameOver();
        }
    }

    public void ClearPendingOrders()
    {
        PendingOrdersList.Clear();
    }

    public void EndDay()
    {
        foreach (var employee in EmployeesList)
        {
            employee.PendingOrder = null;
        }

        ClearPendingOrders();
        ReadyOrderList.Clear();

        CustomerManager.Instance.ClearCustomers();

        foreach (var tray in trays)
        {
            tray.GetComponent<Tray>().ClearOrder();
        }
        foreach (var target in CustomerManager.Instance.TargetList)
        {
            target.IsOccupied = false;
        }
    }
}