using BurgerPunk.Movement;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BurgerPunk.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI killCount;
        [SerializeField] private TextMeshProUGUI dayCount;
        [SerializeField] private TextMeshProUGUI moneyMade;
        [SerializeField] private TextMeshProUGUI structuresBuilt;
        [SerializeField] Button backToMainMenu;
        public void GameOver()
        {
            FindFirstObjectByType<FirstPersonController>().DisableController();

            Time.timeScale = 0f;

            gameOverPanel.SetActive(true);

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

        private void OnEnable()
        {
            GameManager.Instance.uiIsOpen = true;
        }

        private void OnDisable()
        {
            GameManager.Instance.uiIsOpen = false;
        }
    }
}