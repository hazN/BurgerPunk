using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurgerPunk.Player
{
    public class Tray : MonoBehaviour
    {
        [SerializeField] private List<GameObject> trayItems = new List<GameObject>();
        private PendingOrder currentOrder;
        private void Start()
        {
            ClearAll();
        }

        public void EnableItem(bool enable, FoodTypes food)
        {
            trayItems[(int)food].SetActive(enable);
        }

        public void ClearAll()
        {
            foreach (var item in trayItems)
            {
                item.SetActive(false);
            }
        }

        public List<GameObject> GetTrayItems()
        {
            return trayItems;
        }

        public void AssignOrder(PendingOrder order)
        {
            ClearOrder();
            currentOrder = order;
            foreach (var item in currentOrder.OrderItemsList)
            {
                EnableItem(true, item.Type);
            }
        }

        public PendingOrder GetCurrentOrder()
        {
            return currentOrder;
        }

        public void ClearOrder()
        {
            currentOrder = null;
            foreach (var item in trayItems)
            {
                item.SetActive(false);
            }
        }
    }
}