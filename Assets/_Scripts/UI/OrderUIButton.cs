using TMPro;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class OrderUIButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderText;

        public void SetOrderText(string text)
        {
            orderText.text = text;
        }
    }
}