using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public int damage = 3;
    public float lifeTime = 5f;
    public float speed = 4f;

    public Vector2 direction = Vector2.right;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }

        // Set rotation to match velocity direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Player arrow hit enemy for " + damage);
            }
        }

        // Prevent hitting Player/Bow
        if (!other.CompareTag("Player") && !other.name.Contains("Bow"))
        {
            Destroy(gameObject);
        }
    }
}
