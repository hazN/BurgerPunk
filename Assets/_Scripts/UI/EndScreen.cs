using BurgerPunk.Movement;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TMP_Text descriptionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnEnable()
    {
        SetDescription();
        FadeUI.Instance.FadeFromBlack();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.Instance.ambience.Stop();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDescription()
    {
        descriptionText.text = "PIZZAPUNKS ANNIHILATED: <color=green>" + GameManager.Instance.totalEnemiesDefeated.ToString() + "</color>\n" +
            "ENDLESS RICHES ACQUIRED: <color=green>$" + GameManager.Instance.totalMoneyEarned.ToString() + "</color>\n" +
            "BRICKS PAVED IN YOUR KINGDOM: <color=green>" + GameManager.Instance.totalStructuresBuilt.ToString() + "</color>\n" +
            "PEOPLE LITERALLY SAVED BY BURGERS: <color=green>" + GameManager.Instance.totalCustomersServed.ToString() + "</color>";

        GameManager.Instance.SetUIOpen(true);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
        AudioManager.Instance.menuTheme.Play();
    }
}
