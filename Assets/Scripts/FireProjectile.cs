using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public int damageAmount = 10;   // Default, will be overridden by the spawning script (Kaelgroth.cs)
    public float speed = 15f;       // Default speed, can be adjusted in Inspector for each projectile prefab
    public float lifetime = 3f;     // How long the projectile lives before self-destructing if it hits nothing

    // Optional: if you want a particle effect on impact
    // public GameObject impactEffectPrefab; 

    void Start()
    {
        // Self-destruct after 'lifetime' seconds if it hasn't hit anything relevant
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Moves the projectile forward along its local right (X-axis).
        // Assumes the prefab is oriented correctly at spawn by Kaelgroth's firePoint.rotation.
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(gameObject.name + " hit: " + other.name + " with tag: " + other.tag);
        Debug.Log(gameObject.name + " OnTriggerEnter2D with: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ", Layer: " + LayerMask.LayerToName(other.gameObject.layer) + ")"); // ENSURE THIS IS HERE

        Player player = other.GetComponent<Player>();

        if (player != null) // Check if it's the player
        {
            // Debug.Log("Projectile hit Player.");
            player.TakeDamage(damageAmount);

            // Optional: Instantiate an impact effect
            // if (impactEffectPrefab != null) Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject); // Destroy the projectile after hitting the player
            return; // Exit after processing player hit
        }

        // Check for other tags that should destroy the projectile (e.g., environment)
        // Ensure these objects have colliders and the specified tags.
        if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            // Debug.Log("Projectile hit an environment object: " + other.tag);

            // Optional: Instantiate an impact effect
            // if (impactEffectPrefab != null) Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject); // Destroy the projectile
        }

        // Add other specific interactions here if needed, e.g., if it should be destroyed by other enemy types, etc.
        // Be careful not to destroy it if it hits another projectile from the same source unless intended.
    }
}