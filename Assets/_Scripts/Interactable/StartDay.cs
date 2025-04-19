using UnityEngine;

public class StartDayInteractable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.SetupStartDayInteractable(gameObject.GetComponent<Interactable>());
    }
}
