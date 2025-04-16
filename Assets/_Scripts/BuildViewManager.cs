using UnityEngine;
using UnityEngine.UIElements;

public class BuildViewManager : MonoBehaviour
{
    [SerializeField]
    protected Camera buildCamera;
    private Camera mainCamera;

    //[SerializeField]
    RadialProgress radialProgressBar;

    private bool active = false;
    
    LawnGridManager gridManager;
    BuildUI buildUI;


    public float holdToPlaceDuration = 0.4f;
    Vector3 lockedPosition = Vector3.zero;
    GameObject previewObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<LawnGridManager>();
        buildUI = FindFirstObjectByType<BuildUI>();
        buildUI.OnPreviewChanged += UpdatePreviewObject;
        radialProgressBar = buildUI.radialProgressBar;

        mainCamera = Camera.main;
        radialProgressBar.OnRadialProgressComplete += PlaceObject;
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


                if (!radialProgressBar.IsActive() && Input.GetMouseButtonDown(0))
                {
                    radialProgressBar.StartProgress(holdToPlaceDuration);

                }
            }
            else
            {
                Shader.SetGlobalFloat("_IsValid", 0.0f);
            }
        }
        else
        {
            radialProgressBar.CancelProgress();
        }

        if (radialProgressBar.IsActive() && Input.GetMouseButtonUp(0))
        {
            radialProgressBar.CancelProgress();
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

    void PlaceObject()
    {
        PlaceableObject placeableComponent = previewObject.GetComponent<PlaceableObject>();
        placeableComponent.PlaceObject();
        previewObject = Instantiate(buildUI.objectList.placeableObjects[buildUI.objectIndex].objectPrefab);
    }
}
