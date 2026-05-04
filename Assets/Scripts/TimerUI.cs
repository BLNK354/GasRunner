// TimerUI.cs — Gas Runner
// Manages the in-game countdown timer HUD and triggers fines on expiry.
// Attach to a Canvas GameObject that has a TextMeshPro timer text child.

using UnityEngine;
using TMPro;
using System.Collections;

public class TimerUI : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  INSPECTOR SETTINGS                                                  //
    // ------------------------------------------------------------------ //
    [Header("UI References")]
    public TextMeshProUGUI timerText;    // Drag the Timer TMP text here
    public TextMeshProUGUI fineText;     // Drag the Fine alert TMP text here

    [Header("Timer Settings")]
    public Color normalColor  = Color.white;
    public Color warningColor = Color.yellow;  // Last 30 seconds
    public Color dangerColor  = Color.red;     // Last 10 seconds

    // ------------------------------------------------------------------ //
    //  PRIVATE STATE                                                       //
    // ------------------------------------------------------------------ //
    float timeRemaining;
    bool  isRunning;
    bool  hasExpired;

    // ------------------------------------------------------------------ //
    //  UNITY LIFECYCLE                                                     //
    // ------------------------------------------------------------------ //
    void Start()
    {
        // Get time limit from GameManager based on current level
        timeRemaining = GameManager.Instance != null
            ? GameManager.Instance.GetTimeLimitForLevel(GameManager.Instance.currentLevel)
            : 180f;

        isRunning  = true;
        hasExpired = false;

        UpdateDisplay();
    }

    void Update()
    {
        if (!isRunning || hasExpired) return;
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            hasExpired    = true;
            isRunning     = false;
            OnTimerExpired();
        }

        UpdateDisplay();
    }

    // ------------------------------------------------------------------ //
    //  DISPLAY                                                             //
    // ------------------------------------------------------------------ //
    void UpdateDisplay()
    {
        if (timerText == null) return;

        int mins = Mathf.FloorToInt(timeRemaining / 60f);
        int secs = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{mins:00}:{secs:00}";

        // Color changes as time runs low
        if (timeRemaining <= 10f)
            timerText.color = dangerColor;
        else if (timeRemaining <= 30f)
            timerText.color = warningColor;
        else
            timerText.color = normalColor;
    }

    // ------------------------------------------------------------------ //
    //  TIMER EXPIRED                                                       //
    // ------------------------------------------------------------------ //
    void OnTimerExpired()
    {
        Debug.Log("[Timer] Time's up! Fine applied.");
        GameManager.Instance?.ApplyFine("time limit exceeded");
        GameManager.Instance?.SetState(GameManager.GameState.GameOver);
    }

    // ------------------------------------------------------------------ //
    //  PUBLIC METHODS (called by GameManager / UIManager)                 //
    // ------------------------------------------------------------------ //
    public void PauseTimer()  => isRunning = false;
    public void ResumeTimer() => isRunning = true;

    public void ShowFineAlert(int amount)
    {
        if (fineText != null)
            StartCoroutine(FlashAlert($"FINE: -{amount} pts", Color.red));
    }

    IEnumerator FlashAlert(string message, Color color)
    {
        fineText.text    = message;
        fineText.color   = color;
        fineText.enabled = true;

        // Flash 3 times
        for (int i = 0; i < 3; i++)
        {
            fineText.enabled = true;
            yield return new WaitForSecondsRealtime(0.2f);
            fineText.enabled = false;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    public float TimeRemaining => timeRemaining;
    public bool  HasExpired    => hasExpired;
}