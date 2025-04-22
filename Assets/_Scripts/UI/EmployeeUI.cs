using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeUI : MonoBehaviour
{
    [SerializeField] RectTransform employeePanel1;
    [SerializeField] RectTransform employeePanel2;
    [SerializeField] RectTransform employeePanel3;
    [SerializeField] RectTransform employeePanel4;

    [SerializeField] int[] employeeCosts;
    List<RectTransform> employeePanels;

    GameManager gameManager;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        employeePanels.Add(employeePanel1);
        employeePanels.Add(employeePanel2);
        employeePanels.Add(employeePanel3);
        employeePanels.Add(employeePanel4);
    }


    public void HireEmployee(int index)
    {
        int cost = employeeCosts[index];
        int balance = gameManager.GetBalance();

        if (cost <= balance)
        {
            gameManager.SpendMoney(cost);
            employeePanels[index].gameObject.SetActive(false);
        }
    }
}
