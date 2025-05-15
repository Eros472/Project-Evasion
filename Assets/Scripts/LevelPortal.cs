using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    public string targetSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered portal. Loading scene: " + targetSceneName);

            // Optional: Save inventory
            if (GameManager.Instance != null)
                GameManager.Instance.SavePlayerInventory();

            SceneManager.LoadScene(targetSceneName);
        }
    }
}
