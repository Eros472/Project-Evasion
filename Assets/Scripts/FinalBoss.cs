using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 40;
    public int currentHealth = 40;
    public float moveSpeed = 3f;
    public float chaseLength = 5f;
    public float triggerLength = 1.5f;
    public int contactDamage = 4;

    [Header("Fireball Orbit")]
    public float[] fireballSpeed = { 2.5f, -2.5f };
    public float orbitDistance = 0.5f;
    public Transform[] fireballs;

    [Header("Detection")]
    private Transform playerTransform;
    private bool chasing = false;
    private bool collidingWithPlayer = false;
    private Vector3 startingPosition;
    private Rigidbody2D rb;
    private float playerCollisionCooldown = 0f;

    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("FinalBoss: Player not found in scene!");
    }

    private void Update()
    {
        if (playerTransform == null) return;

        ManualUpdate(Time.time);

        if (playerCollisionCooldown > 0f)
            playerCollisionCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        ManualFixedUpdate(Time.fixedDeltaTime);
    }

    private void ManualUpdate(float time)
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            float t = time * fireballSpeed[i];
            fireballs[i].position = transform.position + new Vector3(
                -Mathf.Cos(t) * orbitDistance,
                Mathf.Sin(t) * orbitDistance,
                0
            );
        }
    }

    private void ManualFixedUpdate(float deltaTime)
    {
        float distanceToPlayer = Vector3.Distance(playerTransform.position, startingPosition);

        if (distanceToPlayer < chaseLength)
        {
            if (distanceToPlayer < triggerLength)
                chasing = true;

            if (chasing && playerCollisionCooldown <= 0f)
            {
                MoveTowards(playerTransform.position, deltaTime);
            }
            else
            {
                MoveTowards(startingPosition, deltaTime);
            }
        }
        else
        {
            MoveTowards(startingPosition, deltaTime);
            chasing = false;
        }
    }

    private void MoveTowards(Vector3 target, float deltaTime)
    {
        Vector3 dir = (target - transform.position).normalized;
        rb.MovePosition(transform.position + dir * moveSpeed * deltaTime);
        FlipSprite(dir.x);
    }

    private void FlipSprite(float directionX)
    {
        if (Mathf.Abs(directionX) > 0.1f)
        {
            transform.localScale = directionX < 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                collidingWithPlayer = true;
                player.TakeDamage(contactDamage, transform.position);
                playerCollisionCooldown = 1.0f;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collidingWithPlayer = false;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("FinalBoss defeated!");
        Destroy(gameObject);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame("Victory", 2f);
        }
    }
}


