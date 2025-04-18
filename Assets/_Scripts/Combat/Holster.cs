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

            guns.Find(g => g.GunID == id).Gun.gameObject.SetActive(true);
        }
        public void EquipGun(string name)
        {
            currentGun?.Gun.gameObject.SetActive(false);

            guns.Find(g => g.GunName == name).Gun.gameObject.SetActive(true);
        }
    }
}