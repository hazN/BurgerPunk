using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectData", menuName = "Scriptable Objects/PlaceableObjectData")]
public class PlaceableObjectData : ScriptableObject
{
    public GameObject objectPrefab;
    public int cost = 0;
    public string objectName = "UNNAMED";
    public string objectDescription = "NO DESCRIPTION";
}
