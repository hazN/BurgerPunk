using BurgerPunk.Movement;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        private FirstPersonController player;
        [SerializeField] private TextMeshProUGUI healthText;
        private void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
            if (player == null)
            {
                //Debug.LogError("Player not found in the scene.");
            }
            player.OnPlayerDamage.AddListener(UpdateHealthText);
        }

        private void UpdateHealthText()
        {
            healthText.text = player.HealthPoints.ToString("F0");
        }
    }
}