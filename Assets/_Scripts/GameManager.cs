using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentDay = 0;
    protected float balance = 0;

    public static GameManager Instance;

    [SerializeField]
    public EnemyDayWaves[] enemyDayWaves;
    Queue<EnemyWave> waveQueue = new Queue<EnemyWave>();

    [SerializeField]
    protected float lengthOfDay = 300.0f; // seconds
    protected float dayTimer = 0.0f;

    private bool dayStarted = false;

    [SerializeField]
    EnemySpawnManager enemySpawnManager;

    public System.Action onDayStarted;
    public System.Action onDayEnded;

    Interactable bellInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int numEnemiesDefeatedThisDay = 0;
    public int numMoneyEarnedThisDay = 0;
    public int numStructuresThisDay = 0;
    public int customersServedThisDay = 0;

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
        // TODO: Remove this
        balance = 10000f;
    }

    public void SetupBellInteractable(Interactable interactable)
    {
        bellInteractable = interactable;
        interactable.OnInteracted += RingBell;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dayStarted) return;

        dayTimer += Time.deltaTime;
        dayTimer = Math.Min(dayTimer, lengthOfDay);

        if (waveQueue.Any() && dayTimer/lengthOfDay > waveQueue.Peek().spawnTime)
        {
            EnemyWave wave = waveQueue.Dequeue();
            enemySpawnManager.SpawnWave(wave);
        }
    }

    public void MoveToNextDay()
    {
        currentDay++;
        numEnemiesDefeatedThisDay = 0;
        numMoneyEarnedThisDay = 0;
        numStructuresThisDay = 0;
        customersServedThisDay = 0;
    }

    public bool IsDayOver()
    {
        if (dayTimer >= lengthOfDay) return true;

        return false;
    }

    void EndDay()
    {
        dayStarted = false;
        dayTimer = 0.0f;
        (FindFirstObjectByType(typeof(EndDayScreen)) as EndDayScreen).gameObject.SetActive(true);
        onDayEnded?.Invoke();
    }

    void RingBell()
    {
        if (dayStarted)
        {
            if (IsDayOver())
            {
                EndDay();
            }

            return;
        }
        onDayStarted?.Invoke();

        Debug.Log("Day " + currentDay + 1 + " started.");
        dayTimer = 0.0f;
        dayStarted = true;
        CustomerManager.Instance.SpawnCustomers(3, 5.0f);
        foreach (EnemyWave wave in enemyDayWaves[currentDay].waves)
        {
            waveQueue.Enqueue(wave);
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
}
