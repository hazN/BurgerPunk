using TMPro;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] TMP_Text dayText;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] ProgressBar progressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDay(int day)
    {
        dayText.text = "DAY: " + day.ToString();
    }

    public void SetMoney(int money)
    {
        moneyText.text = "CASH: $" + money.ToString();
    }

    public void SetProgress(float progress)
    {
        progressBar.SetProgress(progress);
    }
}
