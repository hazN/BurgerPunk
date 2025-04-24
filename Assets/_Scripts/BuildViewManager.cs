using BurgerPunk.Movement;
using UnityEngine;

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

    [SerializeField] Interactable buildViewInteractable;
    PlaceableObject placeableComponent;

    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<LawnGridManager>();
        buildUI = FindAnyObjectByType<BuildUI>(FindObjectsInactive.Include);
        buildUI.OnPreviewChanged += UpdatePreviewObject;
        radialProgressBar = buildUI.radialProgressBar;

        mainCamera = Camera.main;
        radialProgressBar.OnRadialProgressComplete += PlaceObject;

        player = GameObject.FindWithTag("Player");

        if (buildViewInteractable)
        {
            buildViewInteractable.OnInteracted += ActivateBuildView;


        }
        DeactivateBuildView();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!active)
            {
                //ActivateBuildView();
            }
            else
            {
                //DeactivateBuildView();
            }
        }

        if (active)
        {
            int layerMask = LayerMask.GetMask("LawnGrid");

            if (previewObject == null) return;

            Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                previewObject.SetActive(true);

                if (radialProgressBar.IsActive())
                {
                    previewObject.transform.position = lockedPosition;
                }
                else
                {
                    previewObject.transform.position = hit.point;
                }
                
                //Debug.Log("Hit: " + hit.collider.name);
                
            }
            else
            {
                previewObject.SetActive(false);
                
            }

            bool placeable = placeableComponent.IsPlaceable();

            //Debug.Log(ray);

            if (placeable)
            {
                Shader.SetGlobalFloat("_IsValid", 1.0f);


                if (!radialProgressBar.IsActive() && Input.GetMouseButtonDown(0) && previewObject.activeSelf)
                {
                    radialProgressBar.StartProgress(holdToPlaceDuration);
                    lockedPosition = previewObject.transform.position;
                }
            }
            else
            {
                Shader.SetGlobalFloat("_IsValid", 0.0f);
            }
        }

        if (radialProgressBar && radialProgressBar.IsActive() && Input.GetMouseButtonUp(0))
        {
            radialProgressBar.CancelProgress();
        }
    }

    void ActivateBuildView()
    {
        active = true;
        UpdatePreviewObject();
        buildCamera.enabled = true;

        if (mainCamera)
        {
            mainCamera.enabled = false;
        }

        buildUI.gameObject.SetActive(true);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player)
        {
            player.GetComponent<FirstPersonController>().enabled = false;
            //player.SetActive(false);
        }
    }

    public void DeactivateBuildView()
    {
        active = false;
        buildCamera.enabled = false;
        Destroy(previewObject);
        radialProgressBar.CancelProgress();

        if (mainCamera)
        {
            mainCamera.enabled = true;
        }

        buildUI.gameObject.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        if (player)
        {
            //player.SetActive(true);
            player.GetComponent<FirstPersonController>().enabled = true;
        }
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

        placeableComponent = previewObject.GetComponentInChildren<PlaceableObject>();
    }

    void PlaceObject()
    {
        if (!GameManager.Instance.TrySpendMoney(buildUI.objectList.placeableObjects[buildUI.objectIndex].cost))
        {
            radialProgressBar.CancelProgress();
            return;
        }

        placeableComponent.PlaceObject();
        AudioManager.Instance.placeableDropped.Play();
        previewObject = Instantiate(buildUI.objectList.placeableObjects[buildUI.objectIndex].objectPrefab);
        UpdatePreviewObject();
    }
}
