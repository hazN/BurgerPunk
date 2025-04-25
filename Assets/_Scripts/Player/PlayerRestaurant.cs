using BurgerPunk.Combat;
using BurgerPunk.Movement;
using BurgerPunk.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BurgerPunk.Player
{
    public class PlayerRestaurant : MonoBehaviour
    {
        private PendingOrder currentOrder;
        private List<OrderItem> itemsToComplete = new List<OrderItem>();
        private bool isOrderComplete = false;
        [SerializeField] private TextMeshProUGUI orderStatus;
        [SerializeField] private RectTransform orderPanel;
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private Tray tray;
        [SerializeField] private RadialProgress radialProgress;
        private void Awake()
        {
            firstPersonController = gameObject.GetComponent<FirstPersonController>();
        }
        public void ClaimOrder(PendingOrder order)
        {
            OrderUI orderUI = FindFirstObjectByType<OrderUI>(FindObjectsInactive.Include);
            AudioManager.Instance.claimOrder.Play();

            if (orderUI != null)
            {
                orderUI.gameObject.SetActive(false);
            }

            if (currentOrder != null)
            {
                //Debug.Log("Already have an order");
                return;
            }

            isOrderComplete = false;

            currentOrder = order;

            itemsToComplete.Clear();
            foreach (var item in order.OrderItemsList)
            {
                itemsToComplete.Add(item);
                string workstation = "";

                if (item.Type == FoodTypes.Burger)
                {
                    workstation = "[GRILL]";
                }
                else if (item.Type == FoodTypes.Fries)
                {
                    workstation = "[FRYER]";
                }
                else if (item.Type == FoodTypes.Soda)
                {
                    workstation = "[VENDING MACHINE]";
                }

                orderStatus.text += item.Name + " " + workstation + "\n";
            }

            if (currentOrder.Customer.SpotLight != null)
                currentOrder.Customer.SpotLight.SetActive(true);

            currentOrder.Customer.InteractCone.gameObject.SetActive(true);
        }

        private void UpdateOrderStatus()
        {
            orderStatus.text = "";
            if (itemsToComplete.Count == 0)
            {
                orderStatus.text = "Order Complete, hand to Customer!";
                if (!isOrderComplete)
                {
                    AudioManager.Instance.orderComplete.Play();
                }

                isOrderComplete = true;

                // HIGHLIGHT CUSTOMER
                // TODO: Implement highlighting logic
                // currentOrder.Customer.getcomponent highlighter??

                return;
            }
            foreach (var item in itemsToComplete)
            {
                string workstation = "";

                if (item.Type == FoodTypes.Burger)
                {
                    workstation = "[GRILL]";
                }
                else if (item.Type == FoodTypes.Fries)
                {
                    workstation = "[FRYER]";
                }
                else if (item.Type == FoodTypes.Soda)
                {
                    workstation = "[VENDING MACHINE]";
                }

                orderStatus.text += item.Name + " " + workstation + "\n";
            }
        }

        public void Cook(FoodTypes foodType, Equipment equipment)
        {
            StartCoroutine(CookCoroutine(foodType, equipment));
        }

        public bool CanUseEquipment(Equipment equipment)
        {
            if (currentOrder == null)
            {
                return false;
            }
            
            FoodTypes foodType = equipment.equipmentType;

            bool needsItem = false;
            foreach (var item in itemsToComplete)
            {
                if (item.Type == foodType)
                {
                    needsItem = true;
                    break;
                }
            }

            return needsItem;
        }
        private IEnumerator CookCoroutine(FoodTypes foodType, Equipment equipment)
        {
            if (currentOrder == null)
            {
                //Debug.Log("No order to cook for");
                yield break;
            }

            bool needsItem = false;
            foreach (var item in itemsToComplete)
            {
                if (item.Type == foodType)
                {
                    needsItem = true;
                    break;
                }
            }

            if (!needsItem) yield break;

            if (foodType == FoodTypes.Burger)
            {
                AudioManager.Instance.grillSfx.Play();
                if (equipment.particlePosition != null) ParticleManager.Instance.CreateParticleEffect(ParticleManager.Particle.Grill, equipment.particlePosition.transform.position, 10f);
            }
            else if (foodType == FoodTypes.Soda)
            {
                AudioManager.Instance.sodaSfx.Play();
            }
            else if (foodType == FoodTypes.Fries)
            {
                AudioManager.Instance.fryerSfx.Play();
            }


            //Debug.Log("Starting to cook...");
            
            firstPersonController.PartiallyDisableController();
            radialProgress.gameObject.SetActive(true);
            radialProgress.StartProgress(2f);
            yield return new WaitForSeconds(2f);
            radialProgress.gameObject.SetActive(false);
            if (!FindAnyObjectByType<EndDayScreen>(FindObjectsInactive.Include).gameObject.activeSelf)
                firstPersonController.EnableController();
            //Debug.Log("Done cooking!");

            foreach (var item in itemsToComplete)
            {
                if (item.Type == foodType)
                {
                    tray.EnableItem(true, foodType);
                    itemsToComplete.Remove(item);
                    UpdateOrderStatus();
                   
                    break;
                }
            }
        }

        private void Update()
        {
            if (currentOrder == null)
            {
                orderPanel.gameObject.SetActive(false);
            }
            else
            {
                orderPanel.gameObject.SetActive(true);
            }
        }

        public bool IsOrderComplete()
        {
            return isOrderComplete;
        }

        public PendingOrder GetCurrentOrder()
        {
            return currentOrder;
        }

        public void ClearOrder()
        {
            if (currentOrder != null)
            {
                currentOrder = null;
                orderStatus.text = "";
                isOrderComplete = false;
                itemsToComplete.Clear();
                tray.ClearAll();
            }
            else
            {
                //Debug.Log("No order to clear");
            }
        }
        public void EquipCompletedOrder(InteractableTray trayObj, Tray completedTray)
        {
            if (currentOrder != null)
            {
                //Debug.Log("Already have an order, complete it first.");
                return;
            }


            ClaimOrder(completedTray.GetCurrentOrder());

            // Complete all items on the tray
            for (int i = itemsToComplete.Count - 1; i >= 0; i--)
            {
                var item = itemsToComplete[i];
                tray.EnableItem(true, item.Type);
                itemsToComplete.RemoveAt(i);
                UpdateOrderStatus();
            }

            trayObj.enabled = false;
            completedTray.ClearAll();
            completedTray.ClearOrder();
        }    
    }
}