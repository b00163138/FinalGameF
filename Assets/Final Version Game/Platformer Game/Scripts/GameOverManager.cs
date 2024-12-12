using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenu;   // Reference to the Game Over Menu panel
    public Transform playerTransform; // Reference to the player's transform
    public float fallThreshold = -10f; // Y position beyond which the player triggers Game Over

    private bool isGameOver = false; // Prevent multiple triggers of Game Over

    void Update()
    {
        // Check if the player has fallen below the threshold and ensure playerTransform is not null
        if (playerTransform != null && !isGameOver && playerTransform.position.y < fallThreshold)
        {
            TriggerGameOver();
        }
    }

    // Method to trigger Game Over
    public void TriggerGameOver()
    {
        isGameOver = true;
        gameOverMenu.SetActive(true); // Display the Game Over Menu
        Time.timeScale = 0f; // Pause the game
    }

    // Method to restart the current scene
    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    // Coroutine to ensure proper reset
    private IEnumerator RestartGameCoroutine()
    {
        Time.timeScale = 1f; // Resume the time scale
        yield return null; // Wait for one frame to ensure Time.timeScale applies
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the scene
    }

    // Method to load the Main Menu scene
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Resume the time scale
        // Replace "MainMenu" with the actual scene name for the Main Menu
        SceneManager.LoadScene("Menu");
    }
}
