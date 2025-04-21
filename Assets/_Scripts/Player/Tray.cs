using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurgerPunk.Player
{
    public class Tray : MonoBehaviour
    {
        [SerializeField] private List<GameObject> trayItems = new List<GameObject>();

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
    }
}