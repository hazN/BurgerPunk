using UnityEngine;

public class Interactable : MonoBehaviour
{
    public System.Action OnInteracted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Interact()
    {
        OnInteracted.Invoke();
    }
}
