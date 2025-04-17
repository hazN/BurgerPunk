using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct OrderItem
{
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
    public string OrderItemsNames;
    public float TotalCost;
    public List<Transform> MachinesList;

    public PendingOrder(PendingOrder pendingOrder)
    {
        Customer = pendingOrder.Customer;
        Employee = pendingOrder.Employee;
        OrderItemsNames = pendingOrder.OrderItemsNames;
        TotalCost = pendingOrder.TotalCost;
        MachinesList = new List<Transform>();
        MachinesList.AddRange(pendingOrder.MachinesList);
    }
}


public class Restaurant : MonoBehaviour
{
    public static Restaurant Instance { get; private set; } = null;
    public List<RestuarantEquipmentWrapper> EquipmentsList = new List<RestuarantEquipmentWrapper>();
    public List<PendingOrder> PendingOrdersList = new List<PendingOrder>();
    public List<PendingOrder> ReadyOrderList = new List<PendingOrder>();
    public List<EmployeeBehaviour> EmployeesList = new List<EmployeeBehaviour>();
    public float TotalSale = 0f;

    private void Awake()
    {
        if (Instance != null)
            throw new UnityException("There can only be one Restaurant object at a time");
        Instance = this;
    }

    public void SpawnEmployees()
    {

    }

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

    public void OrderWrapUp(EmployeeBehaviour employee)
    {
        foreach (var order in PendingOrdersList)
        {
            if (order.Employee == employee)
            {
                ReadyOrderList.Add(order);
                AssignTask(employee);
                return;
            }
        }
    }

    public void AssignTask(EmployeeBehaviour employee = null)
    {
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
        Debug.Log(employee.PendingOrder.OrderItemsNames + ", " + employee.PendingOrder.MachinesList.Count);
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
                if (string.IsNullOrEmpty(orderStr))
                    orderStr = orderItem.Name;
                else orderStr += ", " + orderItem.Name;

                pendingOrder.MachinesList.Add(item.WorldEntity.transform);
                pendingOrder.TotalCost += orderItem.Cost;
            }
            if (itemOrdered == orderSize)
                break;
        }
        pendingOrder.OrderItemsNames = orderStr;
        pendingOrder.Customer = customer;
        PendingOrdersList.Add(pendingOrder);

        AssignTask();

        customer.OrderStr = orderStr;
        Debug.Log("[Restaurant] Order Received: " + orderStr);
        customer.Animator.SetTrigger(customer.m_HashOrder1);
    }
}


