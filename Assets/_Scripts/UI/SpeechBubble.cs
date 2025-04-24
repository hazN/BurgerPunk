using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    List<Image> items = new List<Image>();
    Sprite[] sprites;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();
    }
    public void Setup()
    {
        if (speechType == SpeechType.Image)
        {
            text.gameObject.SetActive(false);
        }
        sprites = new Sprite[3] { burgerIcon, friesIcon, sodaIcon };
        items.Clear();
        items.Add(item1);
        items.Add(item2);
        items.Add(item3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) return;

        transform.forward = Camera.main.transform.forward;
    }

    void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetOrder(List<FoodTypes> order)
    {
        for (int x = 0; x < items.Count; x++)
        {
            items[x].gameObject.SetActive(false);
        }

        for (int x = 0; x < order.Count; x++)
        {
            int foodIndex = (int) order[x];
            items[foodIndex].gameObject.SetActive(true);
            items[foodIndex].sprite = sprites[foodIndex];
        }
    }
}
