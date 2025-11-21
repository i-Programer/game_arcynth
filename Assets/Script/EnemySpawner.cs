using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadius = 8f;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;

    private int currentEnemies = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {

        if (currentEnemies >= maxEnemies) return;
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies++;
    }

    public void EnemyDied()
    {
        Debug.Log("Enemy died. Current enemies: " + (currentEnemies - 1));
        currentEnemies--;
    }
}
