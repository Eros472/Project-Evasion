using UnityEngine;

public class DaggerHitbox : MonoBehaviour
{
    private PlayerInventory player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerInventory>(); // Finds the player to get currentDamage
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DaggerHitbox triggered by: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            int damage = player != null ? player.currentDamage : 0;
            Debug.Log("Enemy tag confirmed. Dealing " + damage + " damage.");

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy.TakeDamage() called.");
            }
            else
            {
                Debug.LogWarning("Enemy component not found on target!");
            }
        }
    }
}
