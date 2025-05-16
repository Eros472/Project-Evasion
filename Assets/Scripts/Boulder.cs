using UnityEngine;

public class Boulder : MonoBehaviour
{
    public float lifetime = 5f;
    public int damage = 5;
    public float impactDelay = 0.3f; // Delay before damage

    private Animator animator;
    private bool hasTriggered = false;

    private Player pendingDamageTarget;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Boulder missing Animator!");

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        Debug.Log("[Boulder] Triggered by: " + collision.name + " | Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            pendingDamageTarget = collision.GetComponent<Player>()
                ?? collision.GetComponentInParent<Player>()
                ?? collision.GetComponentInChildren<Player>();

            if (pendingDamageTarget != null)
            {
                hasTriggered = true;

                Debug.Log("[Boulder] Player detected — triggering impact.");
                if (animator != null)
                {
                    animator.SetTrigger("BreakMid");
                }

                // Start delayed damage sequence
                StartCoroutine(DelayedImpact());
            }
        }
        else
        {
            // Impacted wall or something else
            if (animator != null)
            {
                animator.SetTrigger("BreakMid");
            }
            Destroy(gameObject, 1.5f);
        }
    }

    private System.Collections.IEnumerator DelayedImpact()
    {
        yield return new WaitForSeconds(impactDelay);

        if (pendingDamageTarget != null)
        {
            Debug.Log("[Boulder] Applying delayed damage to player.");
            pendingDamageTarget.TakeDamage(damage);

            // Optional: Knockback
            Vector2 knockDir = (pendingDamageTarget.transform.position - transform.position).normalized;
            Rigidbody2D rb = pendingDamageTarget.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(knockDir * 2f, ForceMode2D.Impulse);
            }

            pendingDamageTarget = null;
        }

        Destroy(gameObject, 1.0f); // Let impact anim finish
    }
}
