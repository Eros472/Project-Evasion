using UnityEngine;

public class Boulder : MonoBehaviour
{
    public float lifetime = 5f;
    private Animator animator;
    private bool hasTriggered = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Boulder is missing Animator component!");
        }

        // Automatically destroy after full lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log("Boulder hit: " + collision.gameObject.name);

        if (animator != null)
        {
            Debug.Log("Triggering BreakMid...");
            animator.SetTrigger("BreakMid");
        }


        if (collision.CompareTag("Player"))
        {
            Debug.Log("BOULDER HIT PLAYER!");
            Destroy(collision.gameObject);
        }

        // Allow time for BreakMid → Impact animations to play fully
        Destroy(gameObject, 1.2f);  // Increased to ~1.2s to be safe
    }

    // Optional: called by end of Impact animation via AnimationEvent
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
