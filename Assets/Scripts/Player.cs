using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;

    public float knockbackForce = 6f;
    public float knockbackDuration = 0.15f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    public HealthBar healthBar;

    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.currentHealth;
            if (currentHealth <= 0)
            {
                currentHealth = GameManager.Instance.maxHealth;
                GameManager.Instance.currentHealth = currentHealth;
            }
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 moveDelta = new Vector3(x, y, 0);

        if (moveDelta.x > 0)
            transform.localScale = Vector3.one;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", moveDelta.sqrMagnitude);
        transform.Translate(moveDelta * 3f * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (GameManager.Instance != null)
            GameManager.Instance.currentHealth = currentHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        StartCoroutine(ApplyKnockback(sourcePosition));

        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    private IEnumerator ApplyKnockback(Vector2 sourcePosition)
    {
        Vector2 direction = (rb.position - sourcePosition).normalized;
        float pushDistance = 0.4f;
        Vector2 newPosition = rb.position + direction * pushDistance;
        rb.MovePosition(newPosition);
        yield return null;
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (GameManager.Instance != null)
            GameManager.Instance.currentHealth = currentHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }
}




