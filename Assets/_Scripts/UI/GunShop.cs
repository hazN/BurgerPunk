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
        private Holster holster;
        private void Start()
        {
            holster = FindFirstObjectByType<Holster>();
            if (holster == null)
            {
                Debug.LogError("Holster not found");
            }
            refreshShop();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gameObject.SetActive(false);
                FindFirstObjectByType<FirstPersonController>().EnableController();
            }
        }

        private Buff GetRandomBuff()
        {
            Buff buff = new Buff();
            // If we have no unlocked guns, we can't give a buff to a gun so only pick between speed , health
            if (holster.GetRandomUnlockedGun() == null)
            {
                buff.buffType = (Buff.BuffType)UnityEngine.Random.Range(0, 2);
            }
            else
            {
                buff.buffType = (Buff.BuffType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Buff.BuffType)).Length - 1);
            }
            buff.name = "";
            buff.value = 0.01f;

            int currentDay = GameManager.Instance.GetCurrentDay();
            int basePrice = Mathf.RoundToInt(50 + currentDay * 10f);
            float randomFactor = UnityEngine.Random.Range(0.85f, 1.15f);
            buff.price = Mathf.RoundToInt(basePrice * randomFactor);


            switch (buff.buffType)
            {
                case Buff.BuffType.speed:
                    buff.name = "Speed";
                    break;
                case Buff.BuffType.health:
                    buff.name = "Health";
                    break;
                case Buff.BuffType.accuracy:
                    var gunData = holster.GetRandomUnlockedGun();
                    buff.name = gunData.GunName + " Accuracy";
                    buff.gunID = gunData.GunID;
                    //buff.value should be random but very low like 0.005 to 0.02
                    buff.value = UnityEngine.Random.Range(0.005f, 0.02f);
                    break;
                case Buff.BuffType.damage:
                    var gunDataDmg = holster.GetRandomUnlockedGun();
                    buff.name = gunDataDmg.GunName + " Damage";
                    buff.gunID = gunDataDmg.GunID;
                    buff.value = UnityEngine.Random.Range(0.01f, 0.1f);
                    break;
                case Buff.BuffType.firerate:
                    var gunDataFireRate = holster.GetRandomUnlockedGun();
                    buff.name = gunDataFireRate.GunName + " Fire Rate";
                    buff.gunID = gunDataFireRate.GunID;
                    buff.value = UnityEngine.Random.Range(0.01f, 0.1f);
                    break;
            }

            return buff;
        }
        private void OnEnable()
        {
            FindFirstObjectByType<FirstPersonController>().DisableController();
            GameManager.Instance.SetUIOpen(true);
        }

        private void OnDisable()
        {
            FirstPersonController controller = FindFirstObjectByType<FirstPersonController>();
            controller.enabled = true;
            controller.EnableController();
            GameManager.Instance.SetUIOpen(false);
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
                if (child.gameObject.name != "DoNotDelete")
                    Destroy(child.gameObject);
            }

            List<GunData> availableGuns = holster.GetLockedGuns();
            for (int i = 0; i < 4; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0 && availableGuns.Count > 0)
                {
                    // Check for duplicates
                    GunData gun = holster.GetRandomLockedGun();
                    while (gunsInShop.Contains(gun))
                    {
                        gun = holster.GetRandomLockedGun();
                    }

                    availableGuns.Remove(gun);
                    gunsInShop.Add(gun);
                    GameObject item = Instantiate(shopItem, transform);
                    item.GetComponent<ShopItem>().Setup(gun);
                }
                else
                {
                    Buff buff = GetRandomBuff();
                    buffsInShop.Add(buff);
                    GameObject item = Instantiate(shopItem, transform);
                    item.GetComponent<ShopItem>().Setup(buff);
                }
            }
        }
    }
}