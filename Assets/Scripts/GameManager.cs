using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent Game State")]
    public List<WeaponType> savedWeapons = new List<WeaponType>();
    public bool isGameOver = false;

    public int savedHealth = -1;
    public int savedMaxHealth = -1;
    private bool hasInitializedHealth = false;

    public int savedEquippedIndex = -1;
    public bool savedEquippedState = false;

    public bool resetHealthOnSceneLoad = false;  // ✅ Flag to force health reset per scene

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator OnSceneLoadedWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Allow other objects to initialize

        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawn = GameObject.Find("PlayerSpawn");

        if (player != null && spawn != null)
        {
            player.transform.position = spawn.transform.position;
            Debug.Log("[GameManager] Player moved to spawn at: " + spawn.transform.position);
        }
        else
        {
            Debug.Log("[GameManager] Player reposition skipped. This scene likely spawns the player later.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[GameManager] Scene loaded: " + scene.name);
        StartCoroutine(OnSceneLoadedWithDelay());

        if (scene.name == "Forest" || scene.name == "Level2")
        {
            // Set level 2 health
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.SetHealth(100); // Set health for level 2
                Debug.Log("[GameManager] Player health set to 80 for Level 2.");
            }
        }
        else
        {
            // Reset health for other levels or the start of the game
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.SetHealth(50); // Reset to default value
            }
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

            Debug.Log("[GameManager] Saved weapons: " + string.Join(", ", savedWeapons));
            Debug.Log($"[GameManager] Saved equipped slot: {savedEquippedIndex}, equipped: {savedEquippedState}");
        }
    }

    public void RestorePlayerInventory(PlayerInventory inventory)
    {
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
            inventory.inventoryBar.SelectSlot(savedEquippedIndex);
            Debug.Log($"[GameManager] Restored equipped weapon: {savedWeapons[savedEquippedIndex]}");
        }
        else
        {
            Debug.Log("[GameManager] No weapon equipped on restore.");
        }

        Debug.Log("[GameManager] Restored inventory: " + string.Join(", ", savedWeapons));
    }

    public void SavePlayerHealth(int current, int max)
    {
        savedHealth = current;
        savedMaxHealth = max;
        hasInitializedHealth = true;
        Debug.Log($"[GameManager] Saved health: {current}/{max}");
    }

    public void RestorePlayerHealth(out int current, out int max)
    {
        if (resetHealthOnSceneLoad || !hasInitializedHealth)
        {
            current = -1;
            max = -1;
            Debug.Log("[GameManager] Health reset or not initialized. Using inspector-defined values.");
            return;
        }

        current = savedHealth;
        max = savedMaxHealth;
        Debug.Log($"[GameManager] Restoring health: {current}/{max}");
    }

    public void EndGame(string resultMessage, float delay)
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over: " + resultMessage);
            StartCoroutine(DelayedSceneLoad("MainMenu", delay));
        }
    }

    private IEnumerator DelayedSceneLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void SaveAllPlayerState()
    {
        Player player = FindObjectOfType<Player>();
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

        if (player != null)
            SavePlayerHealth(player.GetCurrentHealth(), player.GetMaxHealth());

        if (inventory != null)
            SavePlayerInventory();
    }
}
