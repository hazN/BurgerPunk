using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected int day = 0;
    protected int savings = 0;

    [SerializeField]
    protected EnemyDayWaves[] enemyDayWaves;

    [SerializeField]
    protected float lengthOfDay = 300.0f; // seconds
    [SerializeField]
    protected float dayTimer = 0.0f;

    private bool dayStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        dayTimer += Time.deltaTime;
    }

    void StartDay()
    {
        Debug.Log("Day " + day + " started.");
        dayTimer = 0.0f;
        dayStarted = true;
    }
}
