using BurgerPunk.Combat;
using BurgerPunk.Movement;
using UnityEngine;
using UnityEngine.UI;

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
        Button button;
        private void Awake()
        {
            playerController = FindFirstObjectByType<FirstPersonController>();
            holster = FindFirstObjectByType<Holster>();
        }
        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonPress);
        }
        public void Setup(Buff buff)
        {
            nameText.text = buff.name;
            priceText.text = buff.price.ToString();
            this.buff = buff;
            isBuff = true;
        }

        public void Setup(GunData gun)
        {
            nameText.text = gun.GunName;
            priceText.text = gun.Price.ToString();
            gunData = gun;
            isBuff = false;
        }

        // on button press
        public void OnButtonPress()
        {
            if (!GameManager.Instance.TrySpendMoney(isBuff ? buff.price : gunData.Price))
                return;

            AudioManager.Instance.kaching.Play();
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
                        holster.AddAccuracy(buff.gunID, buff.value);
                        break;
                    case Buff.BuffType.damage:
                        holster.AddDamage(buff.gunID, buff.value);
                        break;
                    case Buff.BuffType.firerate:
                        holster.AddFireRate(buff.gunID, buff.value);
                        break;
                }
            }
            else
            {
                holster.UnlockGun(gunData);
            }
            Destroy(gameObject);
        }

        
    }
}