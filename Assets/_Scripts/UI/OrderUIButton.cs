using TMPro;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class OrderUIButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderText;
        [SerializeField] private int orderId;

        private void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonPress);
        }
        public void SetOrderText(string text)
        {
            orderText.text = text;
        }

        public void OnButtonPress()
        {
            Restaurant.Instance.PendingOrdersList.RemoveAll(x => x.OrderId == orderId);

        }
    }
}