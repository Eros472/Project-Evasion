using UnityEngine;

public class WitchDebugDialogue : MonoBehaviour
{
    [TextArea(2, 4)]
    public string message = "Welcome fellow one to the Town of Mirevale. An evil curse has set upon this town and it is your job to travel to Gloamveil Woods using one of the boats at the market sea deck and defeat the almighty beast: Kaelgroth. Take the ancient dagger to help you throughout your journey. ";

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
