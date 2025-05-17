using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KaelgrothEnemy : Enemy
{
    protected override void Die()
    {
        base.Die(); // Call the base method from Enemy to handle default death logic

        // Additional logic specific to Kaelgroth's death
        Debug.Log("[KaelgrothEnemy] Kaelgroth has been defeated!");

        // Notify the GameManager to handle the victory (you can call your GameManager's method here)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleFinalBossDefeated();
        }

        // If you want to delay before transitioning to the main menu, you can start a coroutine
        StartCoroutine(TransitionToMainMenuAfterDelay());
    }

    private IEnumerator TransitionToMainMenuAfterDelay()
    {
        // Wait for a short period (e.g., the length of the death animation)
        yield return new WaitForSeconds(3f);

        // Transition to Main Menu after the delay
        SceneManager.LoadScene("MainMenu");
    }
}
