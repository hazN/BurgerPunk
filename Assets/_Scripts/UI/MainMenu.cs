
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] SettingsMenu settingsMenu;
    [SerializeField] GameObject title;
    [SerializeField] Tutorial tutorial;

    bool gameStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        settingsMenu.OpenSettings();
    }

    public void OpenTutorial()
    {
        tutorial.gameObject.SetActive(true);

    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            FadeUI.Instance.FadeToBlack(GameManager.Instance.StartGame);
            gameStarted = true;
        }
        
    }
}
