// UIManager.cs — Gas Runner
// Central UI controller: HUD score, fine alerts, pause screen, game over screen.
// Attach to the Canvas root GameObject. Requires TextMeshPro package.

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  SINGLETON                                                           //
    // ------------------------------------------------------------------ //
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ------------------------------------------------------------------ //
    //  INSPECTOR — HUD                                                     //
    // ------------------------------------------------------------------ //
    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI fineAlertText;

    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Game Over Screen")]
    public TextMeshProUGUI goScoreText;
    public TextMeshProUGUI goFinesText;
    public TextMeshProUGUI goNetScoreText;
    public Button          goRestartButton;
    public Button          goMenuButton;

    [Header("Pause Screen")]
    public Button pauseResumeButton;
    public Button pauseMenuButton;

    // ------------------------------------------------------------------ //
    //  UNITY LIFECYCLE                                                     //
    // ------------------------------------------------------------------ //
    void Start()
    {
        // Wire up buttons
        goRestartButton?.onClick.AddListener(() => GameManager.Instance?.RestartLevel());
        goMenuButton?.onClick.AddListener(()    => GameManager.Instance?.GoToMainMenu());
        pauseResumeButton?.onClick.AddListener(() => GameManager.Instance?.TogglePause());
        pauseMenuButton?.onClick.AddListener(()  => GameManager.Instance?.GoToMainMenu());

        ShowHUD();
        UpdateScore(0);

        int level = GameManager.Instance?.currentLevel ?? 1;
        if (levelText != null) levelText.text = $"Day {level}";
    }

    void Update()
    {
        // Show/hide pause panel based on game state
        if (pausePanel != null)
        {
            bool isPaused = GameManager.Instance?.CurrentState == GameManager.GameState.Paused;
            pausePanel.SetActive(isPaused);
        }
    }

    // ------------------------------------------------------------------ //
    //  HUD UPDATES                                                         //
    // ------------------------------------------------------------------ //
    public void UpdateScore(int netScore)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {netScore}";
    }

    public void ShowFineAlert(int amount)
    {
        if (fineAlertText != null)
            StartCoroutine(FlashAlert($"FINE: -{amount} pts", Color.red));
    }

    IEnumerator FlashAlert(string message, Color color)
    {
        fineAlertText.text    = message;
        fineAlertText.color   = color;
        fineAlertText.enabled = true;

        // Flash 3 times
        for (int i = 0; i < 3; i++)
        {
            fineAlertText.enabled = true;
            yield return new WaitForSecondsRealtime(0.2f);
            fineAlertText.enabled = false;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    // ------------------------------------------------------------------ //
    //  PANEL CONTROL                                                       //
    // ------------------------------------------------------------------ //
    public void ShowHUD()
    {
        hudPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }

    public void ShowGameOverScreen(int score, int fines, int netScore)
    {
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);

        if (goScoreText    != null) goScoreText.text    = $"Score: {score}";
        if (goFinesText    != null) goFinesText.text    = $"Fines: -{fines}";
        if (goNetScoreText != null) goNetScoreText.text = $"Net Score: {netScore}";

        Debug.Log($"[UIManager] Game Over screen shown. Net: {netScore}");
    }
}