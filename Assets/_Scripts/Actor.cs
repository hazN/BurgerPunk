using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;

    public event System.Action OnDeath;

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0.0f, health - damage);

        if (health <= 0.0f)
        {
            OnDeath.Invoke();
        }
    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }

    public float GetHealth()
    {
        return health;
    }
}
