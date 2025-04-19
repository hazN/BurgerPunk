using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected int currentDay = 0;
    protected int balance = 0;

    public static GameManager Instance;

    [SerializeField]
    protected EnemyDayWaves[] enemyDayWaves;
    Queue<EnemyWave> waveQueue = new Queue<EnemyWave>();

    [SerializeField]
    protected float lengthOfDay = 300.0f; // seconds
    protected float dayTimer = 0.0f;

    private bool dayStarted = false;

    [SerializeField]
    EnemySpawnManager enemySpawnManager;

    public System.Action onDayStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        // TODO: Remove this from here
        StartDay(); // Testing enemies behaviour
    }

    // Update is called once per frame
    void Update()
    {
        if (!dayStarted) return;

        dayTimer += Time.deltaTime;

        if (waveQueue.Any() && dayTimer/lengthOfDay > waveQueue.Peek().spawnTime)
        {
            EnemyWave wave = waveQueue.Dequeue();
            enemySpawnManager.SpawnWave(wave);
        }
    }

    void StartDay()
    {
        onDayStarted?.Invoke();

        Debug.Log("Day " + currentDay + 1 + " started.");
        dayTimer = 0.0f;
        dayStarted = true;

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

    public int GetBalance()
    {
        return balance;
    }
}
