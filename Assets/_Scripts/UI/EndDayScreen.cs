using BurgerPunk.Movement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndDayScreen : MonoBehaviour
{

    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descriptionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>().DisableController();
        titleText.text = "DAY " + GameManager.Instance.GetCurrentDay().ToString() + " COMPLETE";

        descriptionText.text = "BAD GUYS OBLITERATED: " + GameManager.Instance.numEnemiesDefeatedThisDay.ToString() + "\n" +
            "BURGER BUCKS ACQUIRED: $" + GameManager.Instance.numMoneyEarnedThisDay.ToString() + "\n" +
            "STRUCTURES PLACED: " + GameManager.Instance.numStructuresThisDay.ToString() +"\n" +
            "CUSTOMERS SERVED: " + GameManager.Instance.customersServedThisDay.ToString();

        GameManager.Instance.SetUIOpen(true);
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>().EnableController();
        GameManager.Instance.SetUIOpen(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoNextDay()
    {
        FadeUI.Instance.FadeToBlack();
        
        GameManager.Instance.MoveToNextDay();

        this.gameObject.SetActive(false);
    }
}
