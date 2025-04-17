using System.Collections.Generic;
using UnityEngine;

public class LawnGridManager : MonoBehaviour
{
    int rows = 10;
    int cols = 20;
    List<PlaceableObjectData> placedObjects;

    [SerializeField]    
    float XLength = 100.0f;
    [SerializeField]
    float ZLength = 100.0f;


    public GameObject lawnPlane;

    public bool showDebug = true;

    void Start()
    {
        OnValidate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + new Vector3(0.0f, 0.1f, 0.0f), transform.position + new Vector3(XLength, 0.1f, 0.0f));
        Gizmos.DrawLine(transform.position + new Vector3(0.0f, 0.1f, 0.0f), transform.position + new Vector3(0.0f, 0.1f, ZLength));
        Gizmos.DrawLine(transform.position + new Vector3(XLength, 0.1f, 0.0f), transform.position + new Vector3(XLength, 0.1f, ZLength));
        Gizmos.DrawLine(transform.position + new Vector3(0.0f, 0.1f, ZLength), transform.position + new Vector3(XLength, 0.1f, ZLength));

        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        if (lawnPlane)
        {
            lawnPlane.transform.localScale = new Vector3(XLength / 10.0f, 1.0f, ZLength / 10.0f);
            lawnPlane.transform.position = new Vector3(XLength / 2.0f, 0.0f, ZLength / 2.0f);
        }
    }
}
