using BurgerPunk.Movement;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeUI : MonoBehaviour
{
    [SerializeField] RectTransform employeePanel1;
    [SerializeField] RectTransform employeePanel2;
    [SerializeField] RectTransform employeePanel3;
    [SerializeField] RectTransform employeePanel4;

    [SerializeField] float[] employeeCosts;
    List<RectTransform> employeePanels;


    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>().DisableController();
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>().EnableController();
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
            gameObject.SetActive(false);
            FirstPersonController.Instance.EnableController();
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
        }
    }

    public void ExitUI()
    {
        gameObject.SetActive(false);
    }
}
