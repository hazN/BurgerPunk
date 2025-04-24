using System.Collections;
using TMPro;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class RestaurantHealthUI : MonoBehaviour
    {
        private Restaurant restaurant;
        [SerializeField] private TextMeshProUGUI healthText;
        private void Awake()
        {
            restaurant = FindFirstObjectByType<Restaurant>();
            if (restaurant == null)
            {
                Debug.LogError("Restaurant not found in the scene.");
                return;
            }

            restaurant.OnDamageTaken.AddListener(UpdateHealthText);
        }

        private void UpdateHealthText()
        {
            healthText.text = restaurant.HealthPoints.ToString("F0");
        }
    }
}