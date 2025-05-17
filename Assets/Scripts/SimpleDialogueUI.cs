using UnityEngine;
using TMPro; // For TextMeshPro
using System.Collections.Generic; // For Queue

public class SimpleDialogueUI : MonoBehaviour
{
    public static SimpleDialogueUI Instance; // Basic Singleton

    public GameObject dialoguePanel;       // Assign your DialogueDisplayPanel from the Inspector
    public TextMeshProUGUI dialogueText;   // Assign your DialogueTextTMP from the Inspector

    public KeyCode advanceKey = KeyCode.Space; // Key to advance dialogue

    private Queue<string> sentencesToDisplay;
    private bool isDialogueActive = false;

    void Awake()
    {
        // Basic Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            // Optional: If this object needs to persist across general scene loads (not usually needed for just UI like this if it's in every scene that needs it, or re-finds references)
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sentencesToDisplay = new Queue<string>();
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // Ensure it's hidden at start
        }
        else
        {
            Debug.LogError("[SimpleDialogueUI] Dialogue Panel not assigned!");
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (dialoguePanel == null || dialogueText == null || lines == null || lines.Length == 0)
        {
            Debug.LogWarning("[SimpleDialogueUI] Cannot start dialogue. UI elements missing or no lines provided.");
            return;
        }

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        sentencesToDisplay.Clear();

        foreach (string sentence in lines)
        {
            sentencesToDisplay.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentencesToDisplay.Count == 0)
        {
            EndDialogue();
            return;
        }
        string currentSentence = sentencesToDisplay.Dequeue();
        dialogueText.text = currentSentence;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        dialogueText.text = ""; // Clear the text
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(advanceKey))
        {
            DisplayNextSentence();
        }
    }
}