using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemySpawner[] spawnPoints;
    public Transform[] targetPoint;

    public event System.Action OnEnemySpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(EnemySpawnInfo enemy)
    {
        for (int x = 0; x < enemy.numToSpawn; ++x)
        {
            GameObject enemyObject = Instantiate(enemy.enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform);
            EnemyBehaviour enemyBehaviour = enemyObject.GetComponent<EnemyBehaviour>();
            enemyBehaviour.TargetPoint = targetPoint[Random.Range(0, targetPoint.Length)];
            enemyBehaviour.SpawnerManger = this;

            OnEnemySpawned.Invoke();
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
