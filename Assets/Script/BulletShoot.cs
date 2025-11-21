using UnityEngine;

public class BulletShoot : MonoBehaviour
{
    public float lifetime = 3f;
    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after 3 seconds
    }
}
