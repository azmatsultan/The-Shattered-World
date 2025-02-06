using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{

    public GameObject Winscreen;
    public GameObject WinScreenMultiplayerP1;
    public GameObject WinScreenMultiplayerP2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.isMultiPlayer)
            {
                WinScreenMultiplayerP1.SetActive(true);
                WinScreenMultiplayerP2.SetActive(true);
            }
            else
            {
                Winscreen.SetActive(true);
            }
            
            StartCoroutine(CallWithDelay(1f)); // 2-second delay

            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private IEnumerator CallWithDelay(float delay)
    {
        GameManager.Instance.LevelCompleted();
        yield return new WaitForSeconds(delay); // Wait for the specified time
        
        ReturnToMainMenu(); // Call your target function
    }

    public void ReturnToMainMenu()
    {
        // Clear any static variables or game states if necessary
        // Example: GameManager.score = 0;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
