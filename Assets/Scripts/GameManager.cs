using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameActive = true;
    public float score = 0;
    public float roundTime = 180f; // 3 minutes for 1st run
    public int currentRound = 1;

    // 🔥 NEW (Win Condition)
    public int stationsReached = 0;
    public int totalStations = 2;
    private bool gameWon = false;

    void Awake() => Instance = this;

    void Update()
    {
        if (!isGameActive) return;

        score += Time.deltaTime * 10;
        roundTime -= Time.deltaTime;

        // OPTIONAL: Only go next round if NOT yet won
        if (roundTime <= 0 && !gameWon)
        {
            NextRound();
        }
    }

    // ⛽ Called when player reaches a gas station
    public void ReachStation()
    {
        if (!isGameActive) return;

        stationsReached++;
        Debug.Log("Stations reached: " + stationsReached);

        if (stationsReached >= totalStations && !gameWon)
        {
            gameWon = true;
            WinGame();
        }
    }

    void WinGame()
    {
        isGameActive = false;

        Debug.Log("CONGRATS! You reached all gas stations!");

        UIManager.Instance.ShowWinScreen(); // make sure this exists
        Time.timeScale = 0;
    }

    public void NextRound()
    {
        currentRound++;
        roundTime = 120f; // 2 minutes for next runs
    }

    public void GameOver(string reason)
    {
        isGameActive = false;
        UIManager.Instance.ShowGameOver(reason);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}