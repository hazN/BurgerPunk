using BurgerPunk.Combat;
using BurgerPunk.Movement;
using BurgerPunk.Player;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace BurgerPunk
{
    public class Equipment : Interactable
    {
        private PlayerRestaurant playerRestaurant;
        private Holster holster;
        private void Awake()
        {
            playerRestaurant = FindFirstObjectByType<PlayerRestaurant>();
            holster = FindFirstObjectByType<Holster>();
        }
        [SerializeField] private FoodTypes equipmentType;

        public override void Interact()
        {
            // Check if the player is holding the tray
            if (holster.GetCurrentGun().GunName == "Tray")
            {
                if(Vector3.Distance(FirstPersonController.Instance.gameObject.transform.position, transform.position) < 2f)
                {
                    playerRestaurant.Cook(equipmentType);
                }
                else
                {
                    Debug.Log("Player is too far");
                }
            }
            else
            {
                Debug.Log("Need to be holding a tray to use this equipment. Please equip the tray first.");
            }
        }
    }
}