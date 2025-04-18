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

        public Gun Gun => gun;
        public string GunName => gunName;
        public int GunID => gunID;
    }

    public class Holster : MonoBehaviour
    {
        [SerializeField] List<GunData> guns = new List<GunData>();
        private GunData currentGun;
        private void Awake()
        {
            foreach (GunData gunData in guns)
            {
                gunData.Gun.gameObject.SetActive(false);
            }
            EquipGun(2);
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
            int currentIndex = guns.IndexOf(currentGun);
            if (currentIndex < guns.Count - 1)
            {
                EquipGun(guns[currentIndex + 1].GunID);
            }
            else
            {
                EquipGun(guns[0].GunID);
            }
        }

        public void PreviousGun()
        {
            int currentIndex = guns.IndexOf(currentGun);
            if (currentIndex > 0)
            {
                EquipGun(guns[currentIndex - 1].GunID);
            }
            else
            {
                EquipGun(guns[guns.Count - 1].GunID);
            }
        }
    }
}