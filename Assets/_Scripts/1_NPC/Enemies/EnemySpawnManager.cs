using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    EnemySpawner[] spawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(EnemySpawnInfo enemy)
    {
        for (int x = 0; x < enemy.numToSpawn; ++x)
        {
            Instantiate(enemy.enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform);
        }
    }

    public void SpawnWave(EnemyWave wave)
    {
        foreach (EnemySpawnInfo enemy in wave.enemiesToSpawn)
        {
            SpawnEnemy(enemy);
        }
    }
}
