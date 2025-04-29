using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            return;
        }

        PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Fallback assignment
        if (floatingTextManager == null)
            floatingTextManager = FindObjectOfType<FloatingTextManager>();
    }
    //Resources

    //References
    public Player player;
    public RectTransform hitpointBar;
    public GameObject hud;
    public GameObject menu;

    //public weapon weapon..
    public FloatingTextManager floatingTextManager;

    // Logic

    // Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // On Scene Loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        //deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // Save state
        public void SaveState()
    {
        string s = "";
        s += "0" + "|";
        
        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;
        
        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        if (data.Length < 4)
        {
            Debug.LogWarning("Invalid save data, clearing SaveState.");
            PlayerPrefs.DeleteKey("SaveState");
            return;
        }

         // Re-link the player in new scene
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();

        // Re-link floatingTextManager in the new scene
        if (floatingTextManager == null)
            floatingTextManager = FindObjectOfType<FloatingTextManager>();
    }
}
