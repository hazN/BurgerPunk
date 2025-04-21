using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event System.Action OnInteracted;
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {
        OnInteracted?.Invoke();

        if (gameObjects.Count > 0)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
