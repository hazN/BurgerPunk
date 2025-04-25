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
        [SerializeField] public FoodTypes equipmentType;

        public override void Interact()
        {
            // Check if the player is holding the tray
            if (holster.GetCurrentGun().GunName == "Tray")
            {
                playerRestaurant.Cook(equipmentType, this);
            }
            else
            {
                if (playerRestaurant.CanUseEquipment(this))
                {
                    holster.EquipGun("Tray");
                    playerRestaurant.Cook(equipmentType, this);
                }
                
                //Debug.Log("Need to be holding a tray to use this equipment. Please equip the tray first.");
            }
        }
    }
}