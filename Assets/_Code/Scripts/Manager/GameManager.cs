using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> players;

    public GameObject gameStartPanel;
    public GameObject gameWonPanel;
    public GameObject gameLostPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Application.targetFrameRate = 60;
        Time.timeScale = 0f;

        gameStartPanel.SetActive(true);
        gameWonPanel.SetActive(false);
        gameLostPanel.SetActive(false);
    }

    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
        { 
            players.Remove(player);
            Destroy(player);
        }

        // Game Over
        if (players.Count == 0)
        {
            GameEnd(true);
        }
    }

    public void GameEnd(bool isPlayerWon)
    {
            Time.timeScale = 0;
        if (isPlayerWon)
            gameWonPanel.SetActive(true);

        else
            gameLostPanel.SetActive(true);
    }

    public void StartGame()
    {
        gameStartPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
