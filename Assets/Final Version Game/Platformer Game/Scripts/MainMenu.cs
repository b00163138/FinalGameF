using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function will be called when the "Play" button is clicked.
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // This function will be called when the "Quit" button is clicked.
    public void QuitGame()
    {
        // Logs a message in the editor for testing purposes.
        Debug.Log("Quit Game");

        // Closes the application (works in builds only).
        Application.Quit();
    }

    // This function will be called when the "Instructions" button is clicked.
    public void GoInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    // This function will be called when the "Back to Menu" button is clicked.
    public void GoBack()
    {
        SceneManager.LoadScene("Menu");
    }

    // This function will be called when the "GitHubRepo" text is clicked.
    public void OpenGitHubRepo()
    {
        // URL of the GitHub Repository
        string githubURL = "https://github.com/ForensicsCyber/Final-Version-Game";

        // Open the URL in the default browser
        Application.OpenURL(githubURL);

        Debug.Log("Redirecting to: " + githubURL);
    }
}
