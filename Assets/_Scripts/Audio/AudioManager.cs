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
    [SerializeField] public AudioSource dayEnd;
    [SerializeField] public AudioSource applause;
    [SerializeField] public AudioSource applauseBig;
    [SerializeField] public AudioSource underAttack;

    [Header("Music")]
    [SerializeField] public AudioSource menuTheme;
    [SerializeField] public AudioSource pregameSong;
    [SerializeField] public AudioSource daySong1;
    [SerializeField] public AudioSource daySong2;
    [SerializeField] public AudioSource daySong3;

    [Header("Player")]
    [SerializeField] AudioSource gunfire;
    [SerializeField] public AudioSource customerServed;
    [SerializeField] public AudioSource alarm;

    [Header("Restaurant")]
    [SerializeField] public AudioSource fryerSfx;
    [SerializeField] public AudioSource grillSfx;
    [SerializeField] public AudioSource sodaSfx;
    [SerializeField] public AudioSource claimOrder;
    [SerializeField] public AudioSource wrongCustomer;
    [SerializeField] public AudioSource orderComplete;

    [Header("Customer")]
    [SerializeField] AudioSource customerBark;
    [SerializeField] AudioClip[] customerBarkClips;

    [Header("Chef")]
    [SerializeField] AudioClip[] chefClaims;
    [SerializeField] AudioClip[] chefComplete;

    [Header("Enemy")]
    [SerializeField] AudioClip[] enemyAttackClips;
    [SerializeField] AudioClip[] enemyDeathClips;
    [SerializeField] AudioClip[] enemyDamagedClips;

    [Header("Ambience")]
    [SerializeField] public AudioSource ambience;

    [Header("Intro")]
    [SerializeField] public AudioSource introSFX;
    [SerializeField] public AudioClip sparkle;
    [SerializeField] public AudioClip notSoFast;
    [SerializeField] public AudioClip smash;
    [SerializeField] public AudioClip jumphim;
    [SerializeField] public AudioClip imDying;

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

    public AudioSource GetDaySong(int day = 0)
    {
        if (day % 3 == 0)
        {
            return daySong2;
        }
        else if (day % 3 == 1)
        {
            return daySong1;
        }
        else
        {
            return daySong3;
        }
    }
    public void PlayRandomCustomerBark()
    {
        customerBark.clip = customerBarkClips[UnityEngine.Random.Range(0, customerBarkClips.Length)];
        customerBark.Play();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Setting button sfx");
        SetButtonSounds();
    }

    public void Gunfire(AudioClip clip)
    {
        gunfire.PlayOneShot(clip);
    }

    public AudioClip GetChefClaimClip()
    {
        return chefClaims[UnityEngine.Random.Range(0, chefClaims.Length)];
    }

    public AudioClip GetChefCompleteClip()
    {
        return chefComplete[UnityEngine.Random.Range(0, chefComplete.Length)];
    }


    public AudioClip GetEnemyAttackClip()
    {
        return enemyAttackClips[UnityEngine.Random.Range(0, enemyAttackClips.Length)];
    }

    public AudioClip GetEnemyDamagedClip()
    {
        return enemyDamagedClips[UnityEngine.Random.Range(0, enemyDamagedClips.Length)];
    }

    public AudioClip GetEnemyDeathClip()
    {
        return enemyDeathClips[UnityEngine.Random.Range(0, enemyDeathClips.Length)];
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
        //Debug.Log("setting master volume " + volume);
        SetVolume("masterVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        //Debug.Log("setting sfx volume " + volume);
        SetVolume("sfxVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        //Debug.Log("setting music volume " + volume);
        SetVolume("musicVolume", volume);
    }

    
}
