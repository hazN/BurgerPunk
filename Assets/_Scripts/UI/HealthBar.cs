using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Actor actorComponent;
    [SerializeField] Image foregroundImage;
    [SerializeField] bool manual = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //actorComponent = GetComponent<Actor>();
    }

    private void Update()
    {
        if (!manual)
        {
            float health = actorComponent.GetHealth();
            float maxHealth = actorComponent.GetMaxHealth();
            float percentage = health / maxHealth;
            foregroundImage.transform.localScale = new Vector3(percentage, 1.0f, 1.0f);
        }

        if (Camera.main == null) return;

        transform.forward = Camera.main.transform.forward;
    }

    public void SetHealth(float healthPercent)
    {
        foregroundImage.transform.localScale = new Vector3(healthPercent, 1.0f, 1.0f);
    }
}
