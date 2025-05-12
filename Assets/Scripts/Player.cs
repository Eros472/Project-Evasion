using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    public float walkMultiplier = 1.0f;
    public float runMultiplier = 1.5f;

    private bool isDead = false;
    public HealthBar healthBar;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        int currentHealth = GameManager.Instance.currentHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(GameManager.Instance.maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveDelta = new Vector3(x, y, 0);

        if (moveDelta.x > 0)
            transform.localScale = Vector3.one;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Speed", moveDelta.sqrMagnitude);

        float speedMultiplier = walkMultiplier;
        if (isRunning)
            speedMultiplier = runMultiplier;
        if (isCrouching)
            speedMultiplier = walkMultiplier * 0.5f;

        animator.speed = isRunning ? 1.5f : 1.0f;

        transform.Translate(moveDelta * speedMultiplier * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        GameManager.Instance.currentHealth -= amount;
        GameManager.Instance.currentHealth = Mathf.Clamp(GameManager.Instance.currentHealth, 0, GameManager.Instance.maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(GameManager.Instance.currentHealth);

        if (GameManager.Instance.currentHealth <= 0)
            StartCoroutine(Die());
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        GameManager.Instance.currentHealth = Mathf.Min(GameManager.Instance.currentHealth + amount, GameManager.Instance.maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(GameManager.Instance.currentHealth);
    }

    private IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
