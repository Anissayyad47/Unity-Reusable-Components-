using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Playing);
    }

    public void SetState(GameState state)
    {
        CurrentState = state;

        switch (state)
        {
            case GameState.Playing:
                ResumeGame();
                break;

            case GameState.Paused:
                PauseGame();
                break;

            case GameState.GameOver:
                HandleGameOver();
                break;

            case GameState.Victory:
                HandleVictory();
                break;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        CurrentState = GameState.Paused;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    public void Victory()
    {
        SetState(GameState.Victory);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void HandleGameOver()
    {
        Time.timeScale = 1f;

        // Game over logic can be added here.
        // Example: Show Game Over UI.
    }

    private void HandleVictory()
    {
        Time.timeScale = 1f;

        // Victory logic can be added here.
        // Example: Show Victory UI.
    }
}
