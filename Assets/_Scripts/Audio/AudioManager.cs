using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetVolume(string param, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.001f, 1.5f)) * 20f;
        audioMixer.SetFloat(param, db);
        //PlayerPrefs.SetFloat(param, value);
    }
    public void SetMasterVolume(float volume)
    {
        SetVolume(masterGroup.name, volume);
    }

    public void SetSfxVolume(float volume)
    {
        SetVolume(sfxGroup.name, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(musicGroup.name, volume);
    }

    
}
