using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected int currentDay = 0;
    protected int savings = 0;

    public static GameManager Instance;

    [SerializeField]
    protected EnemyDayWaves[] enemyDayWaves;
    Queue<EnemyWave> waveQueue;

    [SerializeField]
    protected float lengthOfDay = 300.0f; // seconds
    [SerializeField]
    protected float dayTimer = 0.0f;

    private bool dayStarted = false;

    EnemySpawnManager enemySpawnManager;

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
        enemySpawnManager = FindFirstObjectByType<EnemySpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dayStarted) return;

        dayTimer += Time.deltaTime;

        if (dayTimer/lengthOfDay > waveQueue.Peek().spawnTime)
        {
            EnemyWave wave = waveQueue.Dequeue();
            enemySpawnManager.SpawnWave(wave);
        }
    }

    void StartDay()
    {
        Debug.Log("Day " + currentDay + " started.");
        dayTimer = 0.0f;
        dayStarted = true;

        foreach (EnemyWave wave in enemyDayWaves[currentDay].waves)
        {
            waveQueue.Enqueue(wave);
        }
    }
}
