using UnityEngine;

/// <summary>
/// Parent class for all placeable objects (I hope)
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    bool active = false;
    bool inPlacementMode = true;

    //Collider collider;
    LayerMask overlapLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // collider = GetComponent<Collider>();
        overlapLayer = LayerMask.GetMask("PlaceableObjects");
    }

    public void PlaceObject()
    {
        inPlacementMode = false;
        gameObject.layer = LayerMask.NameToLayer("PlaceableObjects");
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlacementMode)
        {

        }
    }

    public bool IsPlaceable()
    {
        if (!inPlacementMode) return false;

        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 center = bounds.center;
        Vector3 halfExtents = bounds.extents;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation, overlapLayer);

        foreach (var hit in hits)
        {
            if (hit != GetComponent<Collider>())
            {
                Debug.Log("Overlaps with: " + hit.name);
                return false;
            }
        }

        return true;
    }
}
