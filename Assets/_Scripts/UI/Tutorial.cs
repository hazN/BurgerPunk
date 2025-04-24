using BurgerPunk.Movement;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    [SerializeField] RectTransform[] pageArray;
    int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>()?.DisableController();
        SetPages();
        GameManager.Instance.uiIsOpen = true;
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>()?.EnableController();
        GameManager.Instance.uiIsOpen = false;
    }


    public void GoLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex += pageArray.Length;
        }

        SetPages();
    }

    public void GoRight()
    {
        currentIndex++;
        if (currentIndex >= pageArray.Length)
        {
            currentIndex -= pageArray.Length;
        }

        SetPages();
    }

    void SetPages()
    {
        for (int i = 0; i < pageArray.Length; i++)
        {
            pageArray[i].gameObject.SetActive(false);
        }

        pageArray[currentIndex].gameObject.SetActive(true);
    }
    public void CloseTutorial()
    {
        gameObject.SetActive(false);
    }
}
