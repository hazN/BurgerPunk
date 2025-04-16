using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RestaurantEquipmentData", menuName = "Scriptable Objects/Restaurant Equipment Data")]
public class RestaurantEquipmentData : ScriptableObject
{
    public GameObject ObjectPrefab;
    public int Cost = 0;
    public string EquipmentName = "";
    public string EquipmentFunction = "";
    public List<OrderItem> OrderItemsList;
    public bool IsEquipped;
}
