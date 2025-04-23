using BurgerPunk.Movement;
using Unity.VisualScripting;
using UnityEngine;

public class EndDayScreen : MonoBehaviour
{


    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>().DisableController();
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>().EnableController();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoNextDay()
    {
        gameManager.MoveToNextDay();
        this.gameObject.SetActive(false);
    }
}
