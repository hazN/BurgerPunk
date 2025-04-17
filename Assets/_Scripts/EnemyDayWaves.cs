using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int numToSpawn;
}

[System.Serializable]
public class EnemyWave
{
    [Range(0f, 1f)]
    public float spawnTime;
    public bool isBigWave;
    public bool isFinalWave;

    public EnemySpawnInfo[] enemiesToSpawn;
}

[CreateAssetMenu(fileName = "EnemyDayWaves", menuName = "Scriptable Objects/EnemyDayWaves")]
public class EnemyDayWaves : ScriptableObject
{
    public EnemyWave[] waves;
    int day;
}
