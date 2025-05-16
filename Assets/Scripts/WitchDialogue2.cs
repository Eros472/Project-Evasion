using UnityEngine;

public class WitchDebugDialogue2 : MonoBehaviour
{
    [TextArea(2, 4)]
    public string message = "You're now ready to fight the Kaelgroth. Go on out there and conquer that beast! Just take one of the boats near the sea deck. Watch out for the hooded figures-avoid their purple cone of light or else... We Mirevalians believe in you! ";

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Witch says: {message}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to talk to the Witch.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //Debug.Log("You walked away from the Witch.");
        }
    }
}
