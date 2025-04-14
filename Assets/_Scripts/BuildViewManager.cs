using UnityEngine;

public class BuildViewManager : MonoBehaviour
{
    [SerializeField]
    protected Camera buildCamera;
    private Camera mainCamera;

    private bool active = false;
    
    LawnGridManager gridManager;

    [SerializeField]
    protected PlaceableObjectList objectList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<LawnGridManager>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!active)
            {
                active = true;
                ActivateBuildView();
            }
            else
            {
                DeactivateBuildView();
                active = false;
            }
        }

        if (active)
        {
            int layerMask = LayerMask.GetMask("LawnGrid");
            
            Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("Hit: " + hit.collider.name);
            }

            //Debug.Log(ray);
        }
    }

    void ActivateBuildView()
    {
        buildCamera.enabled = true;
        mainCamera.enabled = false;
    }

    void DeactivateBuildView()
    {
        buildCamera.enabled = false;
        mainCamera.enabled = true;
    }
}
