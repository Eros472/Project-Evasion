using UnityEngine;

public class Boulder : MonoBehaviour
{
    public float lifetime = 5f;
    private Animator animator;
    private bool hasTriggered = false;

    public int damage = 5;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Boulder is missing Animator component!");
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log("Boulder hit: " + collision.gameObject.name);

        if (animator != null)
        {
            animator.SetTrigger("BreakMid");
        }

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        Destroy(gameObject, 1f);
    }
}
