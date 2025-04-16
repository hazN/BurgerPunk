using UnityEngine;
namespace BurgerPunk.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;

        public int GetHealth()
        {
            return currentHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                // game over
            }
        }
    }
}