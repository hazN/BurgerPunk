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

        private void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonPress);
        }
        public void SetOrder(string text, PendingOrder order)
        {
            orderText.text = text;
            this.order = order;
        }

        public void OnButtonPress()
        {
            Restaurant.Instance.PlayerOrder = order;
            Restaurant.Instance.PendingOrdersList.Remove(order);
            FindFirstObjectByType<PlayerRestaurant>().ClaimOrder(order);
            Destroy(gameObject);
        }
    }
}