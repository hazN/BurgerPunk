using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BurgerPunk.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI killCount;
        [SerializeField] private TextMeshProUGUI dayCount;
        [SerializeField] private TextMeshProUGUI moneyMade;
        [SerializeField] private TextMeshProUGUI structuresBuilt;
        [SerializeField] Button backToMainMenu;
        private void OnEnable()
        {
            backToMainMenu.onClick.AddListener(BackToMainMenu);

            killCount. text = GameManager.Instance.totalEnemiesDefeated.ToString();
            dayCount.text = GameManager.Instance.currentDay.ToString();
            moneyMade.text = GameManager.Instance.totalMoneyEarned.ToString();
            structuresBuilt.text = GameManager.Instance.totalStructuresBuilt.ToString();
        }

        private void BackToMainMenu()
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}