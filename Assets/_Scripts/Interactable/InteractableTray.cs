using BurgerPunk.Combat;
using BurgerPunk.Movement;
using BurgerPunk.Player;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace BurgerPunk
{
    public class InteractableTray : Interactable
    {
        private PlayerRestaurant playerRestaurant;
        private Holster holster;
        private Tray tray;
        private void Awake()
        {
            playerRestaurant = FindFirstObjectByType<PlayerRestaurant>();
            holster = FindFirstObjectByType<Holster>();
            tray = GetComponent<Tray>();
        }

        public override void Interact()
        {
            // Check if the player is holding the tray
            if (holster.GetCurrentGun().GunName == "Tray")
            {
                if (tray.GetCurrentOrder() != null)
                    playerRestaurant.EquipCompletedOrder(this, tray);
            }
            else
            {
                //Debug.Log("Need to be holding a tray to use this equipment. Please equip the tray first.");
            }
        }
    }
}