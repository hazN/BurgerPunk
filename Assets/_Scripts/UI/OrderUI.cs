using BurgerPunk.Movement;
using System.Collections;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class OrderUI : MonoBehaviour
    {
        [SerializeField] private GameObject orderUIButton;
        private Restaurant restaurant;
        void Start()
        {
            restaurant = Restaurant.Instance;
            if (restaurant == null)
            {
                Debug.LogError("Restaurant instance is null");
                return;
            }
            restaurant.OnRefreshUI.AddListener(RefreshUI);
        }
        private void OnEnable()
        {
            FindFirstObjectByType<FirstPersonController>().DisableController();
            RefreshUI();
        }

        private void OnDisable()
        {
            FindFirstObjectByType<FirstPersonController>().EnableController();
        }

        public void RefreshUI()
        {
            foreach (Transform child in transform)
            {
                if (child.name != "DoNotDelete")
                {
                    Destroy(child.gameObject);
                }
            }

            if (restaurant.PendingOrdersList.Count > 0)
            {
                // Create a button for each order
                foreach (var order in restaurant.PendingOrdersList)
                {
                    // Make sure its not claimed
                    if (order.Employee != null)
                        continue;

                    string orderText = "";

                    foreach (var item in order.OrderItemsList)
                    {
                        orderText += item.Name + "\n";
                    }

                    GameObject orderButton = Instantiate(orderUIButton, transform);
                    orderButton.GetComponent<OrderUIButton>().SetOrder(orderText, order);
                   
                }
            }
            else
            {
                Debug.Log("No pending orders");
            }

        }
        public void CloseUI()
        {
            FindFirstObjectByType<FirstPersonController>().EnableController();
            gameObject.SetActive(false);
        }
    }
}