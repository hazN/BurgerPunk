using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    Actor actorComponent;
    [SerializeField] Image foregroundImage;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actorComponent = GetComponent<Actor>();
    }

    private void Update()
    {
        float health = actorComponent.GetHealth();
        float maxHealth = actorComponent.GetMaxHealth();
        float percentage = health / maxHealth;
        foregroundImage.transform.localScale = new Vector3(percentage, 1.0f, 1.0f);
    }
}
