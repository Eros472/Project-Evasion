using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 3;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            target?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
