using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle fullscreenToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMasterVolume()
    {
        
    }

    public void OpenSettings()
    {
        Time.timeScale = 0.0f;
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
