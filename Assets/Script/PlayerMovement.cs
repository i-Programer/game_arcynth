using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movSpeed = 5f;      // Player speed
    private Rigidbody2D rb;

    private Vector2 movement;        // Input movement vector
    private Vector2 mousePos;        // Mouse world position

    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- Movement Input ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // Prevent diagonal speed boost

        // --- Mouse Position ---
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // --- Rotation ---
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle; // Rotate the Rigidbody2D to face the mouse

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // --- Apply Movement ---
        rb.linearVelocity = movement * movSpeed;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, rb.position, Quaternion.identity);
        Vector2 direction = (mousePos - rb.position).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

        // Rotate bullet for visuals
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
