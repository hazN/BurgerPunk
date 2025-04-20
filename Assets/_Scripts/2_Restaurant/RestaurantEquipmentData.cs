using System.Collections.Generic;
using UnityEngine;

public enum FoodTypes : int
{
    Burger = 0,
    Fries = 1,
    Soda = 2,
}


[CreateAssetMenu(fileName = "RestaurantEquipmentData", menuName = "Scriptable Objects/Restaurant Equipment Data")]
public class RestaurantEquipmentData : ScriptableObject
{
    public GameObject ObjectPrefab;
    public FoodTypes Type;
    public float Cost = 0;
    public string EquipmentName = "";
    public string EquipmentFunction = "";
    public List<OrderItem> OrderItemsList;
    public bool IsEquipped;
}
