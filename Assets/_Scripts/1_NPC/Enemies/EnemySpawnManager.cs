using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemySpawner[] spawnPoints;
    public Transform[] targetPoint;

    public event System.Action OnEnemySpawned;

    int numEnemiesAlive = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int NumEnemiesAlive()
    {
        return numEnemiesAlive;
    }

    public void OnEnemyDeath()
    {
        numEnemiesAlive--;
    }

    public void SpawnEnemy(EnemySpawnInfo enemy)
    {
        for (int x = 0; x < enemy.numToSpawn; ++x)
        {
            GameObject enemyObject = Instantiate(enemy.enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform);
            EnemyBehaviour enemyBehaviour = enemyObject.GetComponent<EnemyBehaviour>();
            Actor actor = enemyObject.GetComponent<Actor>();

            actor.OnDeath -= OnEnemyDeath;
            actor.OnDeath += OnEnemyDeath;
            enemyBehaviour.TargetPoint = targetPoint[Random.Range(0, targetPoint.Length)];
            enemyBehaviour.SpawnerManger = this;

            OnEnemySpawned?.Invoke();
            numEnemiesAlive++;
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
