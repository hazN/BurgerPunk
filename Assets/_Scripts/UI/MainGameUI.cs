using TMPro;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] TMP_Text dayText;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] ProgressBar progressBar;
    
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        if (gameManager != null)
        {
            SetDay(gameManager.GetCurrentDay());
            SetProgress(gameManager.GetDayProgress());
            SetMoney(gameManager.GetBalance());
        }
    }

    public void SetDay(int day)
    {
        dayText.text = "DAY: " + day.ToString() + "/7";
    }

    public void SetMoney(float money)
    {
        moneyText.text = "CASH: $" + money.ToString();
    }

    public void SetProgress(float progress)
    {
        progressBar.SetProgress(progress);
    }
}
