using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [TextArea(3, 5)] // Makes it easier to type dialogue in the Inspector
    public string[] dialogueLines;
    public KeyCode interactionKey = KeyCode.F; // Key to press to talk to NPC

    private bool playerIsInRange = false;

    // Optional: A simple UI element to show player they can interact
    // public GameObject interactionPrompt; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your Player GameObject is tagged "Player"
        {
            playerIsInRange = true;
            // if (interactionPrompt != null) interactionPrompt.SetActive(true);
            Debug.Log("Player entered range of NPC: " + gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            // if (interactionPrompt != null) interactionPrompt.SetActive(false);
            Debug.Log("Player left range of NPC: " + gameObject.name);

            // Optional: If player walks away while dialogue is active, end it.
            // if (SimpleDialogueUI.Instance != null && SimpleDialogueUI.Instance.IsDialogueActive())
            // {
            //    SimpleDialogueUI.Instance.EndDialogue();
            // }
        }
    }

    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(interactionKey))
        {
            if (SimpleDialogueUI.Instance != null && !SimpleDialogueUI.Instance.IsDialogueActive())
            {
                Debug.Log("Starting dialogue with NPC: " + gameObject.name);
                SimpleDialogueUI.Instance.StartDialogue(dialogueLines);
            }
        }
    }
}