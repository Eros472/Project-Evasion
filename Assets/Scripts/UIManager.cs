using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public HealthBar healthBar;
    public InventoryBar inventoryBar;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find only root objects in current scene
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            // Search for HealthBar only in the current scene hierarchy
            HealthBar foundBar = obj.GetComponentInChildren<HealthBar>(true);
            if (foundBar != null)
            {
                healthBar = foundBar;
                Debug.Log("[UIManager] ✅ Rebound HealthBar from *scene* object.");
                break;
            }
        }

        if (healthBar == null)
            Debug.LogWarning("[UIManager] ❌ HealthBar not found in current scene.");
    }

}
