using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class SpeechBubble : MonoBehaviour
{
    public enum SpeechType { Image, Text };
    public SpeechType speechType = SpeechType.Image;

    [SerializeField] Image item1;
    [SerializeField] Image item2;
    [SerializeField] Image item3;

    [SerializeField] Sprite burgerIcon;
    [SerializeField] Sprite friesIcon;
    [SerializeField] Sprite sodaIcon;
    [SerializeField] Sprite icecreamIcon;
    [SerializeField] TMP_Text text;
    List<Image> items;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (speechType == SpeechType.Image)
        {
            text.enabled = false;
        }
        items = new List<Image>();
        items.Add(item1);
        items.Add(item2);
        items.Add(item3);
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }

    void SetText(string text)
    {

    }

    void SetOrder(List<FoodTypes> order)
    {
        for (int x = 0; x < items.Count; x++) 
        {
            items[x].enabled = false;
        }

        for (int x = 0; x < order.Count; x++)
        {
            items[x].enabled = true;

            if (order[x] == FoodTypes.Burger)
            {
                items[x].sprite = burgerIcon;
            }
            else if (order[x] == FoodTypes.Fries)
            {
                items[x].sprite = friesIcon;
            }
            else if (order[x] == FoodTypes.Soda)
            {
                items[x].sprite = sodaIcon;
            }
        }
    }
}
