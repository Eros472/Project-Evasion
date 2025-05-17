using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;
    public GameObject bloodEffectPrefab; // Assign a different prefab per enemy
    public Color bloodColor = Color.red;

    private Animator animator; // Optional, for animations
    protected bool isDead = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // Optional: Will be null if no Animator is attached
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (bloodEffectPrefab != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            BloodEffect bloodScript = blood.GetComponent<BloodEffect>(); // Assuming you have a BloodEffect script
            if (bloodScript != null)
            {
                bloodScript.SetColor(bloodColor);
            }
        }

        if (currentHealth <= 0 && !isDead) // Ensure Die is only called once
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return; // Safety check if Die() was somehow called again

        isDead = true;
        Debug.Log($"[Enemy Die] Base Die() called for {gameObject.name}. isDead set to true.");

        // Attempt to play "Die" animation only if an Animator exists and has the "Die" trigger
        if (animator != null)
        {
            bool dieParameterExists = false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    dieParameterExists = true;
                    break;
                }
            }

            if (dieParameterExists)
            {
                animator.SetTrigger("Die");
                Debug.Log($"[Enemy Die] Triggering 'Die' animation for {gameObject.name}.");
            }
            else
            {
                Debug.LogWarning($"[Enemy Die] Animator on {gameObject.name} does not have a 'Die' Trigger parameter. Skipping SetTrigger.", gameObject);
            }
        }

        // Generic death behavior
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (this.enabled)
        {
            this.enabled = false; // Disable the Enemy script component itself
        }

        // Conditionally destroy, so Kaelgroth (which loads a scene) isn't destroyed here
        if (!(this is Kaelgroth))
        {
            Debug.Log($"[Enemy Die] {gameObject.name} is not Kaelgroth, scheduling Destroy().");
            Destroy(gameObject, 1f); // Adjust delay as needed for simple enemy death effects
        }
        else
        {
            Debug.LogWarning($"[Enemy Die] {gameObject.name} IS Kaelgroth, SKIPPING Destroy() from Enemy.cs.");
        }
    }
}