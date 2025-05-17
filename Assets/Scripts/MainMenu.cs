using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // In your Main Menu script, when "New Game" is clicked:
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PrepareForNewGame();
        }
        Time.timeScale = 1f; // Also ensure time scale is reset
        SceneManager.LoadScene("Main"); // Or your first playable scene name
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Play button clicked!");
    }

    public void QuitGame()
    {
        Debug.Log("Thank you for playing.");
        Application.Quit();
    }
}
