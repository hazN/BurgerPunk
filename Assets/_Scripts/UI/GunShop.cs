using BurgerPunk.Combat;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace BurgerPunk.UI
{
    public struct Buff
    {
        public string name;
        public float value;
        public Gun gun;
        public BuffType buffType;
        public int price;
        public enum BuffType { speed, health, accuracy, damage, firerate}

    }
    public class GunShop : MonoBehaviour
    {
        [SerializeField] private GameObject shopItem;
        [SerializeField] private List<Buff> buffsInShop = new List<Buff>();
        [SerializeField] private List<GunData> gunsInShop = new List<GunData>();
        private void Awake()
        {
            // Destroy all children
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void Start()
        {
            // unlock mouse
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            for (int i = 0; i < 4; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    Buff buff = GetRandomBuff();
                    buffsInShop.Add(buff);
                    GameObject item = Instantiate(shopItem, transform);
                    item.GetComponent<ShopItem>().Setup(buff);
                }
                else
                {
                    GunData gun = FindFirstObjectByType<Holster>().GetRandomLockedGun();
                    gunsInShop.Add(gun);
                    GameObject item = Instantiate(shopItem, transform);
                    item.GetComponent<ShopItem>().Setup(gun);
                }

            }
        }

        private Buff GetRandomBuff()
        {
            Buff buff = new Buff();
            buff.buffType = (Buff.BuffType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Buff.BuffType)).Length);

            buff.value = 0.01f;
            buff.price = UnityEngine.Random.Range(50, 500);

            switch (buff.buffType)
            {
                case Buff.BuffType.speed:
                    buff.name = "Speed";
                    break;
                case Buff.BuffType.health:
                    buff.name = "Health";
                    break;
                case Buff.BuffType.accuracy:
                    buff.gun = FindFirstObjectByType<Holster>().GetRandomUnlockedGun().Gun;
                    buff.name = buff.gun.name + " Accuracy";
                    break;
                case Buff.BuffType.damage:
                    buff.gun = FindFirstObjectByType<Holster>().GetRandomUnlockedGun().Gun;
                    buff.name = buff.gun.name + " Damage";
                    break;
                case Buff.BuffType.firerate:
                    buff.gun = FindFirstObjectByType<Holster>().GetRandomUnlockedGun().Gun;
                    buff.name = buff.gun.name + " Fire Rate";
                    break;
            }

            return buff;
        }
        private void OnEnable()
        {
            // unlock mouse
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }

        private void OnDisable()
        {
            // lock mouse
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }
}