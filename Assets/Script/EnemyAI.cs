using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemySpawner spawner;

    void start()
    {
        spawner = FindObjectOfType<EnemySpawner>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("Enemy hit by bullet!");
            Destroy(collision.gameObject);
            Destroy(gameObject);
            if (spawner != null)
                spawner.EnemyDied(); // Notify spawner
        }
    }
}
