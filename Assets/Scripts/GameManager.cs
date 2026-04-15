using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public bool isGameActive = true;
    public float score = 0;
    public float roundTime = 180f; // 3 minutes for 1st run
    public int currentRound = 1;

    void Awake() => Instance = this;

    void Update()
    {
        if (!isGameActive) return;

        score += Time.deltaTime * 10;
        roundTime -= Time.deltaTime;

        if (roundTime <= 0)
        {
            NextRound();
        }
    }

    public void NextRound()
    {
        currentRound++;
        roundTime = 120f; // 2 minutes for 2nd run
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