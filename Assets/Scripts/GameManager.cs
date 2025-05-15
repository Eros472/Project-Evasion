using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent Game State")]
    public List<WeaponType> savedWeapons = new List<WeaponType>();
    public bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this before transitioning to another scene
    public void SavePlayerInventory()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            savedWeapons = new List<WeaponType>(inventory.weapons);
            Debug.Log("Saved weapons: " + string.Join(", ", savedWeapons));
        }
    }

    // Optional: Can be used in PlayerInventory on scene load
    public void RestorePlayerInventory(PlayerInventory inventory)
    {
        inventory.weapons = new List<WeaponType>(savedWeapons);
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

    private System.Collections.IEnumerator DelayedSceneLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName); // ensure "MainMenu" is added in Build Settings
    }
}
