using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace BurgerPunk.Combat
{
    [System.Serializable]
    public class GunData
    {
        [SerializeField] private Gun gun;
        [SerializeField] private string gunName;
        [SerializeField] private int gunID;
        [SerializeField] public bool unlocked = false;
        [SerializeField] private int price = 100;
        public Gun Gun => gun;
        public string GunName => gunName;
        public int GunID => gunID;
        public int Price => price;
    }

    public class Holster : MonoBehaviour
    {
        [SerializeField] private List<GunData> guns = new List<GunData>();
        private List<GunData> unlockedGuns = new List<GunData>();
        private GunData currentGun;

        private void Awake()
        {
            unlockedGuns.Clear();
            foreach (GunData gunData in guns)
            {
                gunData.Gun.gameObject.SetActive(false);
                if (gunData.unlocked)
                {
                    unlockedGuns.Add(gunData);
                }
            }

            EquipGun(0);
        }

        public GunData GetCurrentGun()
        {
            return currentGun;
        }

        public void EquipGun(int id)
        {
            currentGun?.Gun.gameObject.SetActive(false);

            currentGun = guns.Find(g => g.GunID == id);
            if (currentGun != null)
            {
                currentGun.Gun.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log($"Gun with name {name} not found in holster.");
            }
        }

        public void EquipGun(string name)
        {
            currentGun?.Gun.gameObject.SetActive(false);

            currentGun = guns.Find(g => g.GunName == name);
            if (currentGun != null)
            {
                currentGun.Gun.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log($"Gun with name {name} not found in holster.");
            }
        }

        public void NextGun()
        {
            if (unlockedGuns.Count == 0)
            {
                //Debug.Log("No unlocked guns to switch to.");
                return;
            }

            int currentIndex = unlockedGuns.IndexOf(currentGun);
            int nextIndex = (currentIndex + 1) % unlockedGuns.Count;
            EquipGun(unlockedGuns[nextIndex].GunID);
        }

        public void PreviousGun()
        {
            if (unlockedGuns.Count == 0)
            {
                //Debug.Log("No unlocked guns to switch to.");
                return;
            }

            int currentIndex = unlockedGuns.IndexOf(currentGun);
            int previousIndex = (currentIndex - 1 + unlockedGuns.Count) % unlockedGuns.Count;
            EquipGun(unlockedGuns[previousIndex].GunID);
        }

        public GunData GetRandomUnlockedGun()
        {
            List<GunData> unlockedGuns = guns.FindAll(g => g.unlocked && g.Gun.IsFireable());
            if (unlockedGuns.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, unlockedGuns.Count);
                return unlockedGuns[randomIndex];
            }
            else
            {
                //Debug.Log("No unlocked guns available.");
                return null;
            }
        }

        public GunData GetRandomLockedGun()
        {
            int level = GameManager.Instance.GetCurrentDay();

            List<GunData> lockedGuns = guns.FindAll(g => !g.unlocked && g.Gun.IsFireable());
            if (lockedGuns.Count == 0)
            {
                //Debug.Log("No locked guns available.");
                return null;
            }

            List<float> weights = new List<float>();
            float totalWeight = 0f;

            foreach (var gun in lockedGuns)
            {
                float price = gun.Price;
                float weight = 1f / Mathf.Pow(price, 1f - Mathf.Clamp01(level * 0.05f));

                weights.Add(weight);
                totalWeight += weight;
            }

            float rand = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < lockedGuns.Count; i++)
            {
                cumulative += weights[i];
                if (rand <= cumulative)
                    return lockedGuns[i];
            }

            return lockedGuns[lockedGuns.Count - 1];
        }

        public void AddAccuracy(int gunId, float value)
        {
            var g = guns.Find(g => g.GunID == gunId);
            if (g != null)
            {
                g.Gun.AddAccuracy(value);
            }
            else
            {
                //Debug.Log($"Gun with ID {gunId} not found.");
            }
        }

        public void AddDamage(int gunId, float value)
        {
            var g = guns.Find(g => g.GunID == gunId);
            if (g != null)
            {
                g.Gun.AddDamage(value);
            }
            else
            {
                //Debug.Log($"Gun with ID {gunId} not found.");
            }
        }

        public void AddFireRate(int gunId, float value)
        {
            var g = guns.Find(g => g.GunID == gunId);
            if (g != null)
            {
                g.Gun.AddFireRate(value);
            }
            else
            {
                //Debug.Log($"Gun with ID {gunId} not found.");
            }
        }

        public void UnlockGun(GunData gun)
        {
            var g = guns.Find(g => g.GunID == gun.GunID);
            if (g != null && !g.unlocked)
            {
                g.unlocked = true;
                if (!unlockedGuns.Contains(g))
                {
                    unlockedGuns.Add(g);
                }
            }
        }
        public int AmountOfLockedGuns()
        {
            int count = 0;
            foreach (var gun in guns)
            {
                if (!gun.unlocked)
                {
                    count++;
                }
            }
            return count;
        }

        public List<GunData> GetLockedGuns()
        {
            List<GunData> lockedGuns = new List<GunData>();
            foreach (var gun in guns)
            {
                if (!gun.unlocked)
                {
                    lockedGuns.Add(gun);
                }
            }
            return lockedGuns;
        }
    }
}