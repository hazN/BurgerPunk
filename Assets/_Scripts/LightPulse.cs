using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;

    private Light myLight;
    private float baseIntensity;

    void Start()
    {
        myLight = GetComponent<Light>();
        baseIntensity = myLight.intensity;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // ranges from 0 to 1
        myLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}