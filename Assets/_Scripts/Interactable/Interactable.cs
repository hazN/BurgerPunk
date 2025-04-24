using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event System.Action OnInteracted;
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();
    [SerializeField] private GameObject speechBubble;

    [SerializeField] private AudioSource interactSfx;
    [SerializeField] private GameObject particlePosition;
    [SerializeField] private ParticleManager.Particle particleType;

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
        if (interactSfx != null)
        {
            interactSfx.Play();
        }

        if(particlePosition != null)
        {
            ParticleManager.Instance.CreateParticleEffect(particleType, particlePosition.transform.position, 5f);
        }

        if (gameObjects.Count > 0)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void EnableText()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
        }
    }

    public void DisableText()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(false);
        }
    }
}
