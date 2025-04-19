using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField]
    Button goLeft;

    [SerializeField]
    Button goRight;

    [SerializeField]
    TMP_Text objectName;

    [SerializeField]
    TMP_Text objectDescription;

    [SerializeField]
    public PlaceableObjectList objectList;


    [SerializeField]
    public RadialProgress radialProgressBar;

    public int objectIndex;

    public event System.Action OnPreviewChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectIndex = 0;
        UpdatePreviewNameAndDescription();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoLeft()
    {
        objectIndex--;
        
        if (objectIndex < 0)
        {
            objectIndex += objectList.placeableObjects.Length;
        }
        UpdatePreviewNameAndDescription();

        OnPreviewChanged.Invoke();
    }

    public void GoRight()
    {
        objectIndex++;

        if (objectIndex >= objectList.placeableObjects.Length)
        {
            objectIndex -= objectList.placeableObjects.Length;
        }
        UpdatePreviewNameAndDescription();

        OnPreviewChanged.Invoke();
    }

    void UpdatePreviewNameAndDescription()
    {
        objectName.text = objectList.placeableObjects[objectIndex].objectName;
        objectDescription.text = objectList.placeableObjects[objectIndex].objectDescription;
    }
}
