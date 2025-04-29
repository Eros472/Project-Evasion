using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public Transform target; // Player's position or enemy to aim at
    public GameObject arrowPrefab; // The arrow prefab to be shot
    public Transform bowPosition; // Position where the arrow will spawn (near bow)
    public float arrowSpeed = 10f; // Speed of the arrow
    public float attackCooldown = 2f; // Time between attacks
    public int arrowDamage = 10; // Damage dealt by arrow

    private float nextAttackTime = 0f;

    void Update()
    {
        if (target != null)
        {
            AimAtTarget();
            if (Time.time >= nextAttackTime)
            {
                ShootArrow();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void AimAtTarget()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
        // Rotate only on the z-axis for aiming
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Flip the Goblin based on direction
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);  // Facing right
        else
            transform.localScale = new Vector3(-1, 1, 1);  // Facing left
    }

    void ShootArrow()
    {
        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, bowPosition.position, bowPosition.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Set Rigidbody2D to be kinematic initially
            rb.isKinematic = true;

            // Calculate direction towards the target
            Vector2 direction = (target.position - transform.position).normalized;

            // Allow physics to control the arrow once we set its velocity
            rb.isKinematic = false;
            rb.velocity = direction * arrowSpeed; // Set arrow velocity
        }

        // Destroy the arrow after 5 seconds if it doesn't hit anything
        Destroy(arrow, 5f);
    }

    // Detect collision and apply damage
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming the player is tagged as "Player"
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                //player.TakeDamage(arrowDamage); // Call TakeDamage on the Player script
            }
            Destroy(gameObject); // Destroy arrow on impact
        }
    }
}
