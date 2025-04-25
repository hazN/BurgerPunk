using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DayOverPrompt : MonoBehaviour
{
    float timer = 31.0f;
    bool countdownActive = false;
    [SerializeField] TMP_Text promptText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!countdownActive) return;

        timer -= Time.deltaTime;

        promptText.text = "DAY COMPLETE! YOU HAVE <size=125%>" + ((int)timer).ToString() + "</size> SECONDS TO SHOP AT THE TRUCK OUTSIDE...";

        if (timer <= 0.0f)
        {
            GameManager.Instance.EndDay();
            countdownActive = false;
        }
    }

    public void StartCountDown()
    {
        timer = 31.0f;
        countdownActive = true;
    }
}
