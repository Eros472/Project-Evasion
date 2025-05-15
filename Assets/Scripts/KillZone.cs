using UnityEngine;

public class KillZone : MonoBehaviour
{
    private bool playerInside = false;
    private Player player;
    private PlayerSafeState safe;

    private float killDelay = 0.25f;
    private float timer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.GetComponent<Player>();
            safe = other.GetComponent<PlayerSafeState>();
            timer = 0f; // start grace period timer
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
            safe = null;
        }
    }

    private void Update()
    {
        if (!playerInside || player == null)
            return;

        timer += Time.deltaTime;

        if (timer >= killDelay)
        {
            // We only kill the player if they're still not in a SafeZone at the end of the delay
            if (safe == null || !safe.IsInSafeZone)
            {
                Debug.Log("KillZone: Player not in SafeZone after delay. Triggering death.");
                player.TakeDamage(player.maxHealth, transform.position);
                playerInside = false; // prevent multiple deaths
            }
            else
            {
                // Still alive — in a safe zone
                Debug.Log("KillZone: Player survived because they're now in a SafeZone.");
                timer = 0f; // Reset delay so it doesn't auto-kill later
            }
        }
    }
}





