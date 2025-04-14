using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectList", menuName = "Scriptable Objects/PlaceableObjectList")]
public class PlaceableObjectList : ScriptableObject
{
    public PlaceableObjectData[] placeableObjects;
}
