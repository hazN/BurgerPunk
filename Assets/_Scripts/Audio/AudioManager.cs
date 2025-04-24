using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("UI")]
    [SerializeField] AudioSource buttonClick;
    [SerializeField] public AudioSource placeableDropped;
    [SerializeField] public AudioSource kaching;

    [Header("Music")]
    [SerializeField] public AudioSource menuTheme;
    [SerializeField] public AudioSource pregameSong;
    [SerializeField] public AudioSource daySong1;

    [Header("Player")]
    [SerializeField] AudioSource gunfire;
    [SerializeField] public AudioSource customerServed;
    [SerializeField] public AudioSource alarm;

    [Header("Restaurant")]
    [SerializeField] public AudioSource fryerSfx;
    [SerializeField] public AudioSource grillSfx;
    [SerializeField] public AudioSource sodaSfx;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetButtonSounds();
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (SceneManager.GetActiveScene().name == "TitleScreen")
            {
                menuTheme.Play();
            }
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Setting button sfx");
        SetButtonSounds();
    }

    public void Gunfire(AudioClip clip)
    {
        gunfire.PlayOneShot(clip);
    }

    void SetButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(() => PlayButtonSFX());
            btn.onClick.AddListener(() => PlayButtonSFX());
        }
    }


    void PlayButtonSFX()
    {
        buttonClick.PlayOneShot(buttonClick.clip);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        musicSource.clip = audioClip;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }

    private void SetVolume(string param, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.001f, 1.5f)) * 20f;
        audioMixer.SetFloat(param, db);
        //PlayerPrefs.SetFloat(param, value);
    }
    public void SetMasterVolume(float volume)
    {
        Debug.Log("setting master volume " + volume);
        SetVolume("masterVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        Debug.Log("setting sfx volume " + volume);
        SetVolume("sfxVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log("setting music volume " + volume);
        SetVolume("musicVolume", volume);
    }

    
}
