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
        titleText.text = "DAY" + GameManager.Instance.currentDay.ToString() + " COMPLETE";

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
        GameManager.Instance.MoveToNextDay();
        this.gameObject.SetActive(false);
    }
}
