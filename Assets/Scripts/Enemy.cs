using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject bloodEffectPrefab; // Assign a different prefab per enemy
    public Color bloodColor = Color.red;


    private Animator animator; // Optional, for animations
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // Optional
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (bloodEffectPrefab != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            BloodEffect bloodScript = blood.GetComponent<BloodEffect>();
            if (bloodScript != null)
            {
                bloodScript.SetColor(bloodColor);
            }
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        // Optional: play death animation
        // if (animator != null) animator.SetTrigger("Die");

        // Optional: disable enemy behavior
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // Optional: destroy after delay
        Destroy(gameObject, 1f);
    }
}
