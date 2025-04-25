using UnityEngine;
using TMPro;

public class ColorBlinkTMP : MonoBehaviour
{
    public TMP_Text tmpText;

    [Header("Colors")]
    public Color colorA = Color.red;
    public Color colorB = Color.yellow;

    [Header("Blink Settings")]
    public float blinkSpeed = 1f; // how fast it switches

    private void Reset()
    {
        // Auto-assign TMP_Text if on same object
        tmpText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (tmpText == null) return;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        tmpText.color = Color.Lerp(colorA, colorB, t);
    }
}