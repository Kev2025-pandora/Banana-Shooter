
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text enemiesText;

    [Header("End Screen")]
    public GameObject endPanel;
    public TMP_Text endText;

    [Header("Game Settings")]
    public int maxWaves = 5;

    private bool gameEnded = false;

    void Start()
    {
        // Hide the end screen at the beginning
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("EndPanel is not assigned!");
        }
    }

    public void UpdateGameUI(int currentWave, int enemiesRemaining)
    {
        if (gameEnded)
        {
            return;
        }

        // Update the wave text
        if (waveText != null)
        {
            waveText.text = "WAVE " + currentWave + " / " + maxWaves;
        }

        // Update the enemy counter
        if (enemiesText != null)
        {
            enemiesText.text = "Enemies Remaining: " + enemiesRemaining;
        }
    }

    public void WinGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        ShowEndScreen("YOU WIN!");
    }

    
public void GameOver()
{
    if (gameEnded) return;

    gameEnded = true;

    // Show the Game Over screen
    endPanel.SetActive(true);
    endText.text = "GAME OVER";

    // Unlock the cursor
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

    void ShowEndScreen(string message)
    {
        // Show the end panel
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("EndPanel is not assigned!");
        }

        // Update the end message
        if (endText != null)
        {
            endText.text = message;
        }
        else
        {
            Debug.LogError("EndText is not assigned!");
        }

        // Unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("End screen shown: " + message);
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
