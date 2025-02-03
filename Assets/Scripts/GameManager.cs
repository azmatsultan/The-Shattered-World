using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton Instance

    private bool isPaused = false;
    public GameObject pausePanel;

    [Header("Score System")]
    public TMP_Text scoreText; // Score display in gameplay
    public TMP_Text highScoreText; // High score display in Main Menu
    public TMP_InputField playerNameInput; // Player name input in Main Menu

    public string playerName;
    public int score;
    //public int highScore;
    //public string highScorePlayer;
    public float startTime; // Time when level starts

    private int[] highScores = new int[3];
    private string[] highScoreNames = new string[3];
    private int currentLevel = 0; // Tracks the current level (0, 1, 2)

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject); // Keep GameManager persistent across scenes
    }

    private void Start()
    {
        LoadHighScore();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // If game is running, update the score based on time
        if (!isPaused && SceneManager.GetActiveScene().name != "MainMenu")
        {
            UpdateScore();
        }
    }

    public void CurrentLevel(int level)
    {
        currentLevel = level;
    }

    public void StartGame()
    {
        playerName = playerNameInput.text; // Get player name from input field
        score = 0;
        startTime = Time.time; // Start tracking time
        UpdateScoreUI();
        //SceneManager.LoadScene("GameScene"); // Load your game scene
    }

    public void AddScore(int points)
    {
        score += points; // Add points for tasks like collectibles or enemies
        UpdateScoreUI();
    }

    private void UpdateScore()
    {
        // Calculate score based on time (lower time = higher score)
        float timeElapsed = Time.time - startTime;
        score = Mathf.Max(0, 1000 - Mathf.RoundToInt(timeElapsed * 10)); // Example: Lose 10 points per second
        UpdateScoreUI();
    }

    public void LevelCompleted()
    {
        // Finalize the score before ending the level
        UpdateScore();

        // Check if the new score is a high score
        if (score > highScores[currentLevel])
        {
            highScores[currentLevel] = score;
            highScoreNames[currentLevel] = playerName;
            SaveHighScore();
        }
        highScoreText.text = $"High Score (Level {currentLevel + 1}): {highScoreNames[currentLevel]} - {highScores[currentLevel]}";

        //SceneManager.LoadScene("MainMenu"); // Load Main Menu after level completion
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void LoadHighScore()
    {
        for (int i = 0; i < 3; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("HighScore" + i, 0);
            highScoreNames[i] = PlayerPrefs.GetString("HighScoreName" + i, "N/A");
        }

        highScoreText.text = "High Scores:\n";
        for (int i = 0; i < 3; i++)
        {
            highScoreText.text += $"Level {i + 1}: {highScoreNames[i]} - {highScores[i]}\n";
        }
    }

    private void SaveHighScore()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
            PlayerPrefs.SetString("HighScoreName" + i, highScoreNames[i]);
        }
        PlayerPrefs.Save();
    }
}
