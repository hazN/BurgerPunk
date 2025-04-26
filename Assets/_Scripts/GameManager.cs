using BurgerPunk.Movement;
using BurgerPunk.Player;
using BurgerPunk.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int currentDay = 0;
    protected float balance = 0;

    public static GameManager Instance;

    [SerializeField]
    public EnemyDayWaves[] enemyDayWaves;
    Queue<EnemyWave> waveQueue = new Queue<EnemyWave>();

    [SerializeField] SettingsMenu settingsMenu;
    [SerializeField] EndDayScreen endDayScreen;

    [SerializeField] protected float lengthOfDay = 300.0f; // seconds
    protected float dayTimer = 0.0f;

    public bool dayStarted = false;
    public bool dayActivitiesComplete = false;

    [SerializeField] EnemySpawnManager enemySpawnManager;
    [SerializeField] GameObject gunShop;

    public System.Action onDayStarted;
    public System.Action onDayEnded;


    Interactable bellInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int numEnemiesDefeatedThisDay = 0;
    public int numMoneyEarnedThisDay = 0;
    public int numStructuresThisDay = 0;
    public int customersServedThisDay = 0;

    public int totalEnemiesDefeated = 0;
    public int totalMoneyEarned = 0;
    public int totalStructuresBuilt = 0;
    public int totalCustomersServed = 0;


    public UnityEvent OnWaveSpawned;

    public DayOverPrompt dayOverPrompt;

    public bool uiIsOpen = false;

    public string mainGameScene = "MainScene_Important_Backup_Backup";
    public string introScene = "Intro";
    public string titleGameScene = "TitleScreen";
    public string endGameScene = "EndScreen";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Setup();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Setup()
    {
        dayStarted = false;
        settingsMenu = FindAnyObjectByType<SettingsMenu>(FindObjectsInactive.Include);
        balance = 1000f;

        settingsMenu.Initialize();
    }

    public void SetupBellInteractable(Interactable interactable)
    {
        bellInteractable = interactable;
        interactable.OnInteracted -= RingBell;
        interactable.OnInteracted += RingBell;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !uiIsOpen)
        {
            if (settingsMenu != null)
            {
                if (!settingsMenu.gameObject.activeSelf)
                {
                    settingsMenu.gameObject.SetActive(true);
                    settingsMenu.OpenSettings();
                }
                else
                {
                    settingsMenu.gameObject.SetActive(false);
                }
            }
        }

        if (!dayStarted) return;

        if (IsDayActivitiesComplete())
        {
            AudioManager.Instance.dayEnd.Play();
            dayOverPrompt.gameObject.SetActive(true);
            dayOverPrompt.StartCountDown();
            Restaurant.Instance.ClearPendingOrders();   
            FindFirstObjectByType<FirstPersonController>().HealToMax();
            gunShop.SetActive(true);
            FindAnyObjectByType<GunShop>(FindObjectsInactive.Include).refreshShop();
            dayStarted = false;
            dayActivitiesComplete = true;
            StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.GetDaySong(currentDay), 1.0f));
        }

        dayTimer += Time.deltaTime;
        dayTimer = Math.Min(dayTimer, lengthOfDay);

        if (waveQueue.Any() && dayTimer / lengthOfDay > waveQueue.Peek().spawnTime)
        {
            EnemyWave wave = waveQueue.Dequeue();
            enemySpawnManager.SpawnWave(wave);
            OnWaveSpawned?.Invoke();
            AudioManager.Instance.alarm.Play();
        }
    }

    public void StartIntro()
    {
        SceneManager.LoadScene(introScene);
        FadeUI.Instance.FadeFromBlack();
    }

    public void StartGame()
    {
        numEnemiesDefeatedThisDay = 0;
        numMoneyEarnedThisDay = 0;
        numStructuresThisDay = 0;
        customersServedThisDay = 0;

        totalEnemiesDefeated = 0;
        totalMoneyEarned = 0;
        totalStructuresBuilt = 0;
        totalCustomersServed = 0;
        currentDay = 0;
        balance = 1000f;
        dayTimer = 0.0f;

        dayStarted = false;
        dayActivitiesComplete = false;

        enemySpawnManager = FindAnyObjectByType<EnemySpawnManager>();
        settingsMenu = null;
        SceneManager.LoadScene(mainGameScene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        //FirstPersonController.Instance.EnableController();

        //FindFirstObjectByType<Tutorial>(FindObjectsInactive.Include).gameObject.SetActive(true);
        StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.menuTheme, 3.0f));
        StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.pregameSong, 3.0f));
        StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.ambience, 3.0f));
        FadeUI.Instance.FadeFromBlack();
    }
    public void MoveToNextDay()
    {
        currentDay++;
        numEnemiesDefeatedThisDay = 0;
        numMoneyEarnedThisDay = 0;
        numStructuresThisDay = 0;
        customersServedThisDay = 0;
        dayStarted = false;
        dayActivitiesComplete = false;

        BuildViewManager bvm = FindFirstObjectByType<BuildViewManager>(FindObjectsInactive.Include);
        if (bvm != null)
        {
            bvm.DeactivateBuildView();
            SetUIOpen(false);
            FindFirstObjectByType<MainGameUI>(FindObjectsInactive.Include).gameObject.SetActive(true);
        }

        EmployeeUI eui = FindFirstObjectByType<EmployeeUI>(FindObjectsInactive.Include);
        if (eui != null)
        {
            if (eui.gameObject.activeSelf)
            {
                eui.gameObject.SetActive(false);
            }
        }

        GunShop gs = FindFirstObjectByType<GunShop>(FindObjectsInactive.Include);
        if (gs != null)
        {
            if (gs.gameObject.activeSelf)
            {
                gs.gameObject.SetActive(false);
            }
        }

        if (currentDay == 7)
        {
            FadeUI.Instance.FadeToBlack(() => SceneManager.LoadScene(endGameScene)); 
            return;
        }
        FadeUI.Instance.FadeToBlack();
        FadeUI.Instance.FadeFromBlack();
        StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.pregameSong, 3.0f));
    }


    public bool IsDayActivitiesComplete()
    {
        if (dayTimer >= lengthOfDay && enemySpawnManager.NumEnemiesAlive() == 0) return true;

        return false;
    }

    public void EndDay() // show end screen
    {
        dayOverPrompt.gameObject.SetActive(false);
        Restaurant.Instance.EndDay();
        FindFirstObjectByType<PlayerRestaurant>(FindObjectsInactive.Include).ClearOrder();
        gunShop.SetActive(false);
        dayTimer = 0.0f;
        if (endDayScreen == null)
        {
            endDayScreen = FindAnyObjectByType<EndDayScreen>(FindObjectsInactive.Include);
        }
        endDayScreen.gameObject.SetActive(true);
        AudioManager.Instance.applause.Play();
        onDayEnded?.Invoke();
    }

    void RingBell()
    {
        if (dayActivitiesComplete)
        {
            //EndDay();
            return;
        }

        if (!dayStarted)
        {
            onDayStarted?.Invoke();
            dayActivitiesComplete = false;
            //Debug.Log("Day " + currentDay + 1 + " started.");
            dayTimer = 0.0f;
            dayStarted = true;
            CustomerManager.Instance.SpawnCustomers(CustomerManager.Instance.CustomersList.Count, 2.0f);
            StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.GetDaySong(currentDay), 3.0f));
            StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.pregameSong, 1.5f));

            foreach (EnemyWave wave in enemyDayWaves[currentDay].waves)
            {
                waveQueue.Enqueue(wave);
            }
        }
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == mainGameScene)
        {
            settingsMenu = FindAnyObjectByType<SettingsMenu>(FindObjectsInactive.Include);
            enemySpawnManager = FindAnyObjectByType<EnemySpawnManager>(FindObjectsInactive.Include);
            endDayScreen = FindAnyObjectByType<EndDayScreen>(FindObjectsInactive.Include);
            gunShop = GameObject.FindGameObjectWithTag("GunTruck");
            gunShop.SetActive(false);
            dayOverPrompt = FindAnyObjectByType<DayOverPrompt>(FindObjectsInactive.Include);

            FindFirstObjectByType<Tutorial>(FindObjectsInactive.Include).gameObject.SetActive(true);
            //UnityEngine.Cursor.lockState = CursorLockMode.None;
            //UnityEngine.Cursor.visible = true;

            //FirstPersonController.Instance.DisableController();
        }
    }
    public int GetCurrentDay()
    {
        return currentDay + 1;
    }

    public float GetDayProgress()
    {
        return dayTimer / lengthOfDay;
    }


    public float GetBalance()
    {
        return balance;
    }

    void SpendMoney(float amount)
    {
        balance -= amount;
    }

    public bool TrySpendMoney(float amount)
    {
        if (balance >= amount)
        {
            balance -= amount;
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        numMoneyEarnedThisDay += amount;
        totalMoneyEarned += amount;
    }

    public void EnemyDied()
    {
        numEnemiesDefeatedThisDay++;
        totalEnemiesDefeated++;
    }

    public void SetUIOpen(bool open)
    {
        uiIsOpen = open;
    }
}
