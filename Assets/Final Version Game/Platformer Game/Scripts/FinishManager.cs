using UnityEngine;
using UnityEngine.SceneManagement;  // Required for SceneManager
using UnityEngine.UI;              // Required for Button references

public class FinishManager : MonoBehaviour
{
    // References to the buttons in your Finish Menu panel
    public Button mainMenuButton;
    public Button restartButton;
    public Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the Finish Menu panel is active
        gameObject.SetActive(true);

        // Assign button listeners
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueToNextLevel);
        }
    }

    // Method to go to the Main Menu scene
    void GoToMainMenu()
    {
        // Load the Menu scene when the player clicks the main menu button
        SceneManager.LoadScene("Menu");
    }

    // Method to restart the current level
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Ensure game time is normal
        AudioListener.pause = false; // Resume all audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Method to continue to the next level (Level-2)
    void ContinueToNextLevel()
    {
        // Ensure the game is unpaused
        Time.timeScale = 1f; // Resume the game
        AudioListener.pause = false; // Resume all audio

        // Load Level2
        SceneManager.LoadScene("Level2");
    }
}

