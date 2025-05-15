using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{
    [Tooltip("Choose which scene this portal leads to.")]
    public int sceneIndex = 0;

    [Tooltip("List of available scene names.")]
    public string[] sceneNames;

    private void Start()
    {
        // Ensure the collider is a trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        // Optionally make this object invisible
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneNames.Length > 0 && sceneIndex >= 0 && sceneIndex < sceneNames.Length)
            {
                // Optional: Save player state using GameManager
                //GameManager.Instance.SavePlayerState(other.transform.position);

                // Load scene
                SceneManager.LoadScene(sceneNames[sceneIndex]);
            }
            else
            {
                Debug.LogWarning("ScenePortal: Invalid scene index or scene list is empty.");
            }
        }
    }
}
