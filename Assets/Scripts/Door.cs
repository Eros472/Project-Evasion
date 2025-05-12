using UnityEngine;

public class Door : MonoBehaviour
{
    private bool playerInRange = false;
    private bool isUnlocked = false;

    public void Unlock()
    {
        if (isUnlocked) return;

        Debug.Log("Door unlocked!");
        isUnlocked = true;

        // Add animation/sound or destroy if needed
        GetComponent<Collider2D>().enabled = false;
    }

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
