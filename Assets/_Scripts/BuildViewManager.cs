using UnityEngine;

public class BuildViewManager : MonoBehaviour
{
    [SerializeField]
    protected Camera buildCamera;
    private Camera mainCamera;

    private bool active = false;
    
    LawnGridManager gridManager;
    BuildUI buildUI;

    
    GameObject previewObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<LawnGridManager>();
        buildUI = FindFirstObjectByType<BuildUI>();
        buildUI.OnPreviewChanged += UpdatePreviewObject;

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
                previewObject.SetActive(true);
                previewObject.transform.position = hit.point;
            }
            else
            {
                previewObject.SetActive(false);
            }

            PlaceableObject placeableComponent = previewObject.GetComponent<PlaceableObject>();
            bool placeable = placeableComponent.IsPlaceable();

            //Debug.Log(ray);

            if (placeable)
            {
                Shader.SetGlobalFloat("_IsValid", 1.0f);


                if (Input.GetMouseButtonDown(0))
                {
                    placeableComponent.PlaceObject();
                    previewObject = Instantiate(buildUI.objectList.placeableObjects[buildUI.objectIndex].objectPrefab);

                }
            }
            else
            {
                Shader.SetGlobalFloat("_IsValid", 0.0f);
            }

            
        }
    }

    void ActivateBuildView()
    {
        buildCamera.enabled = true;
        mainCamera.enabled = false;

        UpdatePreviewObject();
    }

    void DeactivateBuildView()
    {
        buildCamera.enabled = false;
        mainCamera.enabled = true;

        Destroy(previewObject);
    }

    void UpdatePreviewObject()
    {
        if (previewObject == null)
        {
            previewObject = Instantiate(buildUI.objectList.placeableObjects[buildUI.objectIndex].objectPrefab);
        }
        else
        {
            Destroy(previewObject);
            previewObject = Instantiate(buildUI.objectList.placeableObjects[buildUI.objectIndex].objectPrefab);
        }
    }
}
