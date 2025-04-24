using BurgerPunk.Movement;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeUI : MonoBehaviour
{
    [SerializeField] RectTransform employeePanel1;
    [SerializeField] RectTransform employeePanel2;
    [SerializeField] RectTransform employeePanel3;
    [SerializeField] RectTransform employeePanel4;
    [SerializeField] TMP_Text employeeText1;
    [SerializeField] TMP_Text employeeText2;
    [SerializeField] TMP_Text employeeText3;
    [SerializeField] TMP_Text employeeText4;

    [SerializeField] float[] employeeCosts;
    List<RectTransform> employeePanels;

    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>().DisableController();
        employeeText1.text = "$" + employeeCosts[0].ToString();
        employeeText2.text = "$" + employeeCosts[1].ToString();
        employeeText3.text = "$" + employeeCosts[2].ToString();
        employeeText4.text = "$" + employeeCosts[3].ToString();

        GameManager.Instance.SetUIOpen(true);
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>().EnableController();

        GameManager.Instance.SetUIOpen(false);
    }

    void Start()
    {
        employeePanels = new List<RectTransform>();
        FirstPersonController.Instance.DisableController();
        employeePanels.Add(employeePanel1);
        employeePanels.Add(employeePanel2);
        employeePanels.Add(employeePanel3);
        employeePanels.Add(employeePanel4);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitUI();
        }
    }


    public void HireEmployee(int index)
    {
        float cost = employeeCosts[index];
        float balance = GameManager.Instance.GetBalance();

        if (cost <= balance)
        {
            GameManager.Instance.TrySpendMoney(cost);
            Restaurant.Instance.SpawnEmployees(index);
            employeePanels[index].gameObject.SetActive(false);
            AudioManager.Instance.kaching.Play();
        }
    }

    public void ExitUI()
    {
        gameObject.SetActive(false);
        FirstPersonController.Instance.EnableController();
    }
}