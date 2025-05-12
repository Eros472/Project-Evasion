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

    public int maxHealth = 20;
    private int currentHealth;
    private bool isDead = false;

    public HealthBar healthBar;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
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

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("MainMenu");
    }
}
