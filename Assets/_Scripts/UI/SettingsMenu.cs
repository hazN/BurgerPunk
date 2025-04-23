using BurgerPunk.Movement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    AudioManager audioManager;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle fullscreenToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void UpdateMasterVolume()
    {
        audioManager.SetMasterVolume(masterVolumeSlider.value);
    }

    public void UpdateSfxVolume()
    {
        audioManager.SetSfxVolume(sfxVolumeSlider.value);
    }


    public void UpdateMusicVolume()
    {
        audioManager.SetMusicVolume(musicVolumeSlider.value);
    }


    public void OpenSettings()
    {
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
        FindFirstObjectByType<FirstPersonController>()?.DisableController();
    }

    private void OnDisable()
    {
        FindFirstObjectByType<FirstPersonController>()?.EnableController();
    }

    public void OpenTutorial()
    {
        GoBack();
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleFullScreen()
    {
        if (fullscreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
}
