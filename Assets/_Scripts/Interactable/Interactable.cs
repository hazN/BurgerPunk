using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event System.Action OnInteracted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        OnInteracted?.Invoke();
    }
}
