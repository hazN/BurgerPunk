using NUnit.Framework;
using System;
using System.Collections.Generic;
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
                Debug.Log($"Gun with name {name} not found in holster.");
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
                Debug.Log($"Gun with name {name} not found in holster.");
            }
        }

        public void NextGun()
        {
            if (unlockedGuns.Count == 0)
            {
                Debug.Log("No unlocked guns to switch to.");
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
                Debug.Log("No unlocked guns to switch to.");
                return;
            }

            int currentIndex = unlockedGuns.IndexOf(currentGun);
            int previousIndex = (currentIndex - 1 + unlockedGuns.Count) % unlockedGuns.Count;
            EquipGun(unlockedGuns[previousIndex].GunID);
        }

        public GunData GetRandomUnlockedGun()
        {
            List<GunData> unlockedGuns = guns.FindAll(g => g.unlocked);
            if (unlockedGuns.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, unlockedGuns.Count);
                return unlockedGuns[randomIndex];
            }
            else
            {
                Debug.Log("No unlocked guns available.");
                return null;
            }
        }

        public GunData GetRandomLockedGun()
        {
            List<GunData> lockedGuns = guns.FindAll(g => !g.unlocked);
            if (lockedGuns.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, lockedGuns.Count);
                return lockedGuns[randomIndex];
            }
            else
            {
                Debug.Log("No locked guns available.");
                return null;
            }
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
                Debug.Log($"Gun with ID {gunId} not found.");
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
                Debug.Log($"Gun with ID {gunId} not found.");
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
                Debug.Log($"Gun with ID {gunId} not found.");
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
    }
}