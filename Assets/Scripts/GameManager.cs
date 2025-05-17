using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Required for List
// using TMPro; // Uncomment if your victory message text is TextMeshPro
// using UnityEngine.UI; // Uncomment if your victory message text is legacy UI Text

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent Game State")]
    public List<WeaponType> savedWeapons = new List<WeaponType>();
    public bool isGameOver = false; // Keep if used elsewhere

    public int savedPlayerCurrentHealth = -1;
    public int savedPlayerMaxHealth = -1;
    private bool playerHealthInitializedByGM = false;

    public int savedEquippedIndex = -1;
    public bool savedEquippedState = false;

    public bool forcePlayerHealthResetOnInitialLoad = true;

    [Header("Victory Sequence Settings")]
    public GameObject victoryPanel; // Assign your UI Panel from the Inspector
    // Example: public TextMeshProUGUI victoryMessageText; // If you want to change text dynamically
    public float victoryMessageDuration = 6.0f; // Duration to show message (5-7 seconds)
    public string mainMenuSceneName = "MainMenu"; // IMPORTANT: Set this to your exact Main Menu scene name

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoadedHandler;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedHandler;
    }

    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");
        StartCoroutine(PositionPlayerAfterSceneLoad());

        // Hide victory panel if it was active from a previous scene (e.g., if DontDestroyOnLoad was on it by mistake)
        if (victoryPanel != null && victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(false);
        }
    }

    private IEnumerator PositionPlayerAfterSceneLoad()
    {
        yield return null; // Wait a frame

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.Find("PlayerSpawn");

        if (playerObject != null && spawnPoint != null)
        {
            playerObject.transform.position = spawnPoint.transform.position;
            Debug.Log($"[GameManager] Player moved to spawn point '{spawnPoint.name}' at: {spawnPoint.transform.position} in scene '{SceneManager.GetActiveScene().name}'.");
        }
        else
        {
            if (playerObject == null) Debug.LogWarning("[GameManager] Player object with tag 'Player' not found for repositioning in scene: " + SceneManager.GetActiveScene().name);
            if (spawnPoint == null) Debug.LogWarning("[GameManager] 'PlayerSpawn' object not found for repositioning in scene: " + SceneManager.GetActiveScene().name);
        }
    }

    public void SavePlayerInventory()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            savedWeapons = new List<WeaponType>(inventory.weapons);
            savedEquippedIndex = inventory.currentWeaponIndex;
            savedEquippedState = inventory.weaponEquipped;
        }
    }

    public void RestorePlayerInventory(PlayerInventory inventory)
    {
        if (inventory == null) return;

        inventory.weapons = new List<WeaponType>(savedWeapons);

        if (UIManager.Instance != null && inventory.inventoryBar != null)
        {
            for (int i = 0; i < savedWeapons.Count; i++)
            {
                inventory.inventoryBar.AddWeaponIcon(i, savedWeapons[i]);
            }
        }

        inventory.currentWeaponIndex = savedEquippedIndex;
        inventory.weaponEquipped = savedEquippedState;

        if (savedEquippedState && savedEquippedIndex >= 0 && savedEquippedIndex < savedWeapons.Count)
        {
            inventory.EquipCurrentWeapon();
            if (inventory.inventoryBar != null) inventory.inventoryBar.SelectSlot(savedEquippedIndex);
        }
    }

    public void SavePlayerHealth(int current, int max)
    {
        savedPlayerCurrentHealth = current;
        savedPlayerMaxHealth = max;
        playerHealthInitializedByGM = true;
        Debug.Log($"[GameManager] Player health state received and saved: {current}/{max}");
    }

    public void RestorePlayerHealth(out int current, out int max)
    {
        if (forcePlayerHealthResetOnInitialLoad || !playerHealthInitializedByGM)
        {
            current = -1;
            max = -1;
            Debug.Log("[GameManager] Instructing Player to use default/scene-specific health on initial load (or GM not yet initialized with player health).");
        }
        else
        {
            current = savedPlayerCurrentHealth;
            max = savedPlayerMaxHealth;
            Debug.Log($"[GameManager] Providing saved player health to Player.Start(): {current}/{max}");
        }
    }

    // --- Victory Sequence Methods ---

    /// <summary>
    /// Called by the defeated final boss to initiate the victory sequence.
    /// </summary>
    public void HandleFinalBossDefeated()
    {
        Debug.Log("[GameManager] Final Boss Defeated! Transitioning to Main Menu...");

        // Skip showing any victory panel or message
        StartCoroutine(ReturnToMainMenuAfterVictory());
    }

    private IEnumerator ReturnToMainMenuAfterVictory()
    {
        // Wait for a few seconds before going to the main menu
        Debug.Log("[GameManager] Waiting before returning to Main Menu...");

        // If you want a delay after the death animation of Kaelgroth, set this to a number (e.g., 5 seconds)
        yield return new WaitForSeconds(5f); // Adjust the delay time as needed

        Debug.Log("[GameManager] Returning to Main Menu...");

        // Load the Main Menu scene directly
        SceneManager.LoadScene("MainMenu");
    }

    public void PrepareForNewGame()
    {
        Debug.LogWarning("[GameManager] Preparing state for a new game.");

        // Reset any game state variables here. Examples:
        // isGameOver = false; // If you have such a flag

        // Crucially for player health on a fresh start:
        savedPlayerCurrentHealth = -1; // Or your player's actual starting default health
        savedPlayerMaxHealth = -1;   // Or your player's actual starting default max health
        playerHealthInitializedByGM = false;
        forcePlayerHealthResetOnInitialLoad = true; // This ensures Player.cs uses defaults

        // Reset inventory if it's managed here
        savedWeapons.Clear();
        savedEquippedIndex = -1;
        savedEquippedState = false;

        // Reset any other flags related to progression, boss defeats, etc.
        // For example, if Kaelgroth's defeat set a global flag, reset it.
    }


}
