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
public struct RestaurantEquipment
{
    public PlaceableObjectData EquipmentData;

    [Tooltip("All the items this equiment can offer")]
    public List<OrderItem> OrderItemsList;
    public bool IsEquipped;
}

public class Restaurant : MonoBehaviour
{
    public static Restaurant Instance { get; private set; } = null;
    public List<RestaurantEquipment> EquipmentsList = new List<RestaurantEquipment>();
    public float TotalSale = 0f;

    private void Awake()
    {
        if (Instance != null)
            throw new UnityException("There can only be one Restaurant object at a time");
        Instance = this;
    }

    public void GetRandomOrder(CustomerBehaviour customer)
    {
        string orderStr = string.Empty;
        int orderSize = EquipmentsList.Count <= 1 ? 1 : Random.Range(1, EquipmentsList.Count + 1);
        int itemOrdered = 0;
        for (int i = 0; i < EquipmentsList.Count; i++)
        {
            var item = EquipmentsList[i];
            if (item.IsEquipped)
            {
                itemOrdered++;
                OrderItem orderItem = item.OrderItemsList[Random.Range(0, item.OrderItemsList.Count)];

                if (string.IsNullOrEmpty(orderStr))
                    orderStr = orderItem.Name;
                else orderStr += ", " + orderItem.Name;
                 
                TotalSale += orderItem.Cost;
            }
            if (itemOrdered == orderSize)
                break;
        }

        customer.Animator.SetTrigger(customer.m_HashOrder1);

        customer.OrderStr = orderStr;
        Debug.Log("[Restaurant] Order Received: " + orderStr);
    }
}


