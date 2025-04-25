using TMPro;
using UnityEngine;

public class FadeTextUI : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float fadeDelay = 0f;

    float timeElapsed = 0;
    TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > fadeTime + fadeDelay)
        {
            Destroy(this);
        }
        if (timeElapsed > fadeDelay)
        {
            text.alpha = 1f - ((timeElapsed - fadeDelay) / fadeTime);
        }
    }
}
