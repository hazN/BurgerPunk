using UnityEngine;

public enum AnimationType { None, Bobbing, Shake }

public class AmbientMotion : MonoBehaviour
{
    [SerializeField] public AnimationType animationType;

    public float amplitude = 0.1f;
    public float frequency = 1.0f;

    private Vector3 cachedLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cachedLocation = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition = cachedLocation + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
        Debug.Log(Time.deltaTime);
        Debug.Log(cachedLocation + Vector3.up * Mathf.Sin(Time.deltaTime * frequency) * amplitude);
    }
}
