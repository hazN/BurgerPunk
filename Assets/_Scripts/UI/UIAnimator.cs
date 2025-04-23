using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public float amplitude = 10f; // how high it moves
    public float frequency = 2f;  // how fast it moves

    private RectTransform rectTransform;
    private Vector2 startPos;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * frequency) * amplitude;
        rectTransform.anchoredPosition = startPos + new Vector2(0f, bob);
    }
}