using BurgerPunk.Combat;
using BurgerPunk.Movement;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private TMPro.TextMeshProUGUI priceText;
        bool isBuff;
        Buff buff;
        GunData gunData;
        private FirstPersonController playerController;
        private Holster holster;
        private void Awake()
        {
            playerController = FindFirstObjectByType<FirstPersonController>();
            holster = FindFirstObjectByType<Holster>();
        }
        public void Setup(Buff buff)
        {
            nameText.text = buff.name;
            priceText.text = buff.price.ToString();
            this.buff = buff;
        }

        public void Setup(GunData gun)
        {
            nameText.text = gun.GunName;
            priceText.text = gun.Price.ToString();
            gunData = gun;
        }

        // on button press
        public void OnButtonPress()
        {
            if (isBuff)
            {
                switch (buff.buffType)
                {
                    case Buff.BuffType.speed:
                        playerController.AddSpeed(buff.value);
                        break;
                    case Buff.BuffType.health:
                        playerController.AddHealth(buff.value);
                        break;
                    case Buff.BuffType.accuracy:
                        holster.AddAccuracy(gunData, buff.value);
                        break;
                    case Buff.BuffType.damage:
                        holster.AddDamage(gunData, buff.value);
                        break;
                    case Buff.BuffType.firerate:
                        holster.AddFireRate(gunData, buff.value);
                        break;
                }
            }
            else
            {
                holster.UnlockGun(gunData);
            }
        }
    }
}