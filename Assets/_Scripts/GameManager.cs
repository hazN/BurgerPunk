using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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
    [SerializeField] protected float lengthOfDay = 300.0f; // seconds
    protected float dayTimer = 0.0f;

    private bool dayStarted = false;
    private bool dayActivitiesComplete = false;

    [SerializeField] EnemySpawnManager enemySpawnManager;

    public System.Action onDayStarted;
    public System.Action onDayEnded;


    Interactable bellInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int numEnemiesDefeatedThisDay = 0;
    public int numMoneyEarnedThisDay = 0;
    public int numStructuresThisDay = 0;
    public int customersServedThisDay = 0;

    [Header("Scenes")]
    [SerializeField] SceneAsset mainMenuScene;
    [SerializeField] SceneAsset mainGameScene;

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
        // TODO: Remove this
        balance = 10000f;
        //fadeUI.gameObject.SetActive(true);
        //fadeUI.FadeFromBlack();
        //FadeUI.Instance.gameObject.SetActive(true);
        //FadeUI.Instance.FadeFromBlack();
    }

    public void SetupBellInteractable(Interactable interactable)
    {
        bellInteractable = interactable;
        interactable.OnInteracted += RingBell;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

        dayTimer += Time.deltaTime;
        dayTimer = Math.Min(dayTimer, lengthOfDay);

        if (waveQueue.Any() && dayTimer/lengthOfDay > waveQueue.Peek().spawnTime)
        {
            EnemyWave wave = waveQueue.Dequeue();
            enemySpawnManager.SpawnWave(wave);
        }

        if (IsDayActivitiesComplete())
        {
            dayStarted = false;
            dayActivitiesComplete = true;
            StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.daySong1, 1.0f));
        }
    }

    public void StartGame()
    {
        enemySpawnManager = FindAnyObjectByType<EnemySpawnManager>();
        settingsMenu = null;
        SceneManager.LoadScene(mainGameScene.name);
        SceneManager.sceneLoaded += OnSceneLoaded;
        

        StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.menuTheme, 3.0f));
        StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.pregameSong, 3.0f));
        FadeUI.Instance.FadeFromBlack();
    }
    public void MoveToNextDay()
    {
        currentDay++;
        numEnemiesDefeatedThisDay = 0;
        numMoneyEarnedThisDay = 0;
        numStructuresThisDay = 0;
        customersServedThisDay = 0;
    }


    public bool IsDayActivitiesComplete()
    {
        if (dayTimer >= lengthOfDay && enemySpawnManager.NumEnemiesAlive() == 0) return true;

        return false;
    }

    void EndDay() // show end screen
    {
        dayTimer = 0.0f;
        (FindFirstObjectByType(typeof(EndDayScreen)) as EndDayScreen).gameObject.SetActive(true);
        onDayEnded?.Invoke();
    }

    void RingBell()
    {
        if (dayActivitiesComplete)
        {
            EndDay();
        }

        if (!dayStarted)
        {
            onDayStarted?.Invoke();

            dayActivitiesComplete = false;
            Debug.Log("Day " + currentDay + 1 + " started.");
            dayTimer = 0.0f;
            dayStarted = true;
            CustomerManager.Instance.SpawnCustomers(3, 5.0f);
            StartCoroutine(AudioFade.FadeIn(AudioManager.Instance.daySong1, 3.0f));
            StartCoroutine(AudioFade.FadeOut(AudioManager.Instance.pregameSong, 1.5f));

            foreach (EnemyWave wave in enemyDayWaves[currentDay].waves)
            {
                waveQueue.Enqueue(wave);
            }
        }
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == mainGameScene.name)
        {
            settingsMenu = FindAnyObjectByType<SettingsMenu>(FindObjectsInactive.Include);
            enemySpawnManager = FindAnyObjectByType<EnemySpawnManager>(FindObjectsInactive.Include);
        }
    }
    public int GetCurrentDay()
    {
        return currentDay;
    }

    public float GetDayProgress()
    {
        return dayTimer / lengthOfDay;
    }

    public void AddToBalance(float amount)
    {
        balance += amount;
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
    }

    public void EnemyDied()
    {
        numEnemiesDefeatedThisDay++;
    }
}
