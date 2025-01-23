using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private bool isPaused = false; // To track if the game is paused
    public GameObject pausePanel; // Assign your Pause UI panel in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Esc key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Resume if already paused
            }
            else
            {
                PauseGame(); // Pause the game
            }
        }
    }

    // Method to pause the game
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); // Show the Pause UI panel
        Time.timeScale = 0f; // Freeze game time

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Method to resume the game
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); // Hide the Pause UI panel
        Time.timeScale = 1f; // Resume game time

        // Lock and hide the cursor (if necessary for gameplay)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        // Clear any static variables or game states if necessary
        // Example: GameManager.score = 0;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    } 

}
