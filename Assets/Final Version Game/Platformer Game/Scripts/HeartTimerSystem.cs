using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartTimerSystem : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Reference to Timer Text
    public GameObject gameOverPanel; // Reference to Game Over Panel
    public Image[] hearts; // Array to hold Heart Images (Full/Empty)
    public Sprite fullHeart; // Sprite for Full Heart
    public Sprite emptyHeart; // Sprite for Empty Heart

    private float totalTime = 180f; // 180 seconds timer
    private int heartIndex = 0; // Tracks which heart to empty next

    void Start()
    {
        // Initialize timer and hearts
        UpdateTimerUI();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        // Reduce time
        totalTime -= Time.deltaTime;
        UpdateTimerUI();

        // Check for 60-second intervals and update hearts
        if (totalTime <= 120 && heartIndex < 1) UpdateHeart();
        if (totalTime <= 60 && heartIndex < 2) UpdateHeart();
        if (totalTime <= 0 && heartIndex < 3) GameOver();
    }

    void UpdateTimerUI()
    {
        // Update the timer text
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        timerText.text = string.Format("TIME : {0:00}:{1:00}", minutes, seconds);
    }

    void UpdateHeart()
    {
        if (heartIndex < hearts.Length)
        {
            hearts[heartIndex].sprite = emptyHeart;
            heartIndex++;
        }
    }

    void GameOver()
    {
        // Set all hearts to empty, activate Game Over panel
        foreach (var heart in hearts)
        {
            heart.sprite = emptyHeart;
        }
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }
}
