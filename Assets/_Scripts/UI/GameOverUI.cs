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
        [SerializeField] private TextMeshProUGUI timeSurvived;
        [SerializeField] private TextMeshProUGUI dayCount;
        [SerializeField] private TextMeshProUGUI moneyMade;
        [SerializeField] Button backToMainMenu;
        private void OnEnable()
        {
            backToMainMenu.onClick.AddListener(BackToMainMenu);

            // todo
        }

        private void BackToMainMenu()
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}