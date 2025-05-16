using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneEntryHandler : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(MovePlayerToSpawn());
    }

    IEnumerator MovePlayerToSpawn()
    {
        // Wait for end of frame to ensure all objects are loaded
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.Find("PlayerSpawn");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            Debug.Log("[SceneEntryHandler] Moved player to PlayerSpawn at " + spawnPoint.transform.position);
        }
        else
        {
            Debug.LogWarning("[SceneEntryHandler] Could not find Player or PlayerSpawn.");
        }
    }
}
