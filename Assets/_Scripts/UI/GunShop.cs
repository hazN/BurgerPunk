using BurgerPunk.Combat;
using BurgerPunk.Movement;
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
        public int gunID;
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
        }

        private void Start()
        {
            refreshShop();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                FindFirstObjectByType<FirstPersonController>().EnableController();
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
                    var gunData = FindFirstObjectByType<Holster>().GetRandomUnlockedGun();
                    buff.name = gunData.GunName + " Accuracy";
                    buff.gunID = gunData.GunID;
                    break;
                case Buff.BuffType.damage:
                    var gunDataDmg = FindFirstObjectByType<Holster>().GetRandomUnlockedGun();
                    buff.name = gunDataDmg.GunName + " Damage";
                    buff.gunID = gunDataDmg.GunID;
                    break;
                case Buff.BuffType.firerate:
                    var gunDataFireRate = FindFirstObjectByType<Holster>().GetRandomUnlockedGun();
                    buff.name = gunDataFireRate.GunName + " Fire Rate";
                    buff.gunID = gunDataFireRate.GunID;
                    break;
            }

            return buff;
        }
        private void OnEnable()
        {
            FindFirstObjectByType<FirstPersonController>().DisableController();
        }

        private void OnDisable()
        {
            FindFirstObjectByType<FirstPersonController>().EnableController();
        }
        public void CloseShop()
        {
            FindFirstObjectByType<FirstPersonController>().EnableController();
            gameObject.SetActive(false);
        }
        public void refreshShop()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "Exit")
                    Destroy(child.gameObject);
            }
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
    }
}