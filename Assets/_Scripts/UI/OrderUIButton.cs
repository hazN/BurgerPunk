using BurgerPunk.Player;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class OrderUIButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderText;
        [SerializeField] private PendingOrder order;
        private PlayerRestaurant playerRestaurant;

        private void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonPress);
            playerRestaurant = FindFirstObjectByType<PlayerRestaurant>();
        }
        public void SetOrder(string text, PendingOrder order)
        {
            orderText.text = text;
            this.order = order;
        }

        public void OnButtonPress()
        {
            // make sure we dont have any orders
            if (playerRestaurant.GetCurrentOrder() != null)
                return;
            Restaurant.Instance.PlayerOrder = order;
            Restaurant.Instance.PendingOrdersList.Remove(order);
            playerRestaurant.ClaimOrder(order);
            Destroy(gameObject);
        }
    }
}