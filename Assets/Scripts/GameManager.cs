// GameManager.cs — Gas Runner
// Core controller: manages game states, level flow, score, and fines.
// Attach this to an empty GameObject named "GameManager" in every scene.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  SINGLETON                                                           //
    // ------------------------------------------------------------------ //
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // Persist across scenes
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ------------------------------------------------------------------ //
    //  GAME STATE                                                          //
    // ------------------------------------------------------------------ //
    public enum GameState { MainMenu, Playing, Paused, QTE, GameOver }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public bool isGameActive => CurrentState == GameState.Playing;

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] State → {newState}");

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.QTE:
                Time.timeScale = 0f;   // Freeze game during QTE/pause
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                OnGameOver();
                break;
        }
    }

    // ------------------------------------------------------------------ //
    //  LEVEL & SCORING                                                     //
    // ------------------------------------------------------------------ //
    [Header("Level Settings")]
    public int currentLevel = 1;

    // Time limits match the proposal: Level 1 = 3min, Level 2 = 2min
    public float[] levelTimeLimits = { 180f, 120f };

    [Header("Score")]
    public int score = 0;
    public int fines = 0;
    public int fineAmount = 50;

    public int NetScore => score - fines;

    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance?.UpdateScore(NetScore);
    }

    public void ApplyFine(string reason = "time limit exceeded")
    {
        fines += fineAmount;
        Debug.Log($"[GameManager] Fine applied: {reason}. Total fines: {fines}");
        UIManager.Instance?.ShowFineAlert(fineAmount);
        UIManager.Instance?.UpdateScore(score - fines);
    }

    public float GetTimeLimitForLevel(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, levelTimeLimits.Length - 1);
        return levelTimeLimits[index];
    }

    // ------------------------------------------------------------------ //
    //  SCENE MANAGEMENT                                                    //
    // ------------------------------------------------------------------ //
    public void StartGame()
    {
        score = 0;
        fines = 0;
        currentLevel = 1;
        SetState(GameState.Playing);
        SceneManager.LoadScene("Level1");
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        if (currentLevel > levelTimeLimits.Length)
        {
            LoadScene("WinScreen");
        }
        else
        {
            LoadScene("Level" + currentLevel);
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ReachStation()
    {
        Debug.Log("[GameManager] Gas station reached");
        AddScore(250);
        LoadNextLevel();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        score = 0;
        fines = 0;
        currentLevel = 1;
        SetState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            CurrentState = GameState.MainMenu;
            Time.timeScale = 1f;
            return;
        }

        if (scene.name.StartsWith("Level"))
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowHUD();
            UIManager.Instance?.UpdateScore(NetScore);
        }
    }

    // ------------------------------------------------------------------ //
    //  GAME OVER                                                           //
    // ------------------------------------------------------------------ //
    void OnGameOver()
    {
        Debug.Log($"[GameManager] Game Over — Score: {score} | Fines: {fines} | Net: {NetScore}");
        UIManager.Instance?.ShowGameOverScreen(score, fines, NetScore);
    }

    public void GameOver(string reason = "Game over")
    {
        Debug.Log($"[GameManager] Game Over: {reason}");
        SetState(GameState.GameOver);
    }

    // ------------------------------------------------------------------ //
    //  PAUSE                                                               //
    // ------------------------------------------------------------------ //
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            SetState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            SetState(GameState.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }
}
