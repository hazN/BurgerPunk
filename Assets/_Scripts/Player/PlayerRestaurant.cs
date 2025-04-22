using BurgerPunk.Movement;
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
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private Tray tray;
        private void Awake()
        {
            firstPersonController = gameObject.GetComponent<FirstPersonController>();
        }
        public void ClaimOrder(PendingOrder order)
        {
            if (currentOrder != null)
            {
                Debug.Log("Already have an order");
                return;
            }

            isOrderComplete = false;

            currentOrder = order;

            itemsToComplete.Clear();
            foreach (var item in order.OrderItemsList)
            {
                itemsToComplete.Add(item);
                orderStatus.text += item.Name + "\n";
            }
        }

        private void UpdateOrderStatus()
        {
            orderStatus.text = "";
            if (itemsToComplete.Count == 0)
            {
                orderStatus.text = "Order Complete, hand to Customer!";
                isOrderComplete = true;
                return;
            }
            foreach (var item in itemsToComplete)
            {
                orderStatus.text += item.Name + "\n";
            }
        }

        public void Cook(FoodTypes foodType)
        {
            StartCoroutine(CookCoroutine(foodType));
        }

        private IEnumerator CookCoroutine(FoodTypes foodType)
        {
            if (currentOrder == null)
            {
                Debug.Log("No order to cook for");
                yield break;
            }

            Debug.Log("Starting to cook...");
            firstPersonController.DisableController();
            yield return new WaitForSeconds(3f);
            firstPersonController.EnableController();
            Debug.Log("Done cooking!");

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
                Debug.Log("No order to clear");
            }
        }
        public void EquipCompletedOrder(InteractableTray trayObj, Tray completedTray)
        {
            if (currentOrder != null)
            {
                Debug.Log("Already have an order, complete it first.");
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