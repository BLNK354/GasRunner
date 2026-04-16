using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gameplay UI")]
    public Slider fuelSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI reasonText;

    [Header("Win UI")] // 🔥 NEW
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    void Awake() => Instance = this;

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        fuelSlider.value = FuelSystem.Instance.currentFuel / FuelSystem.Instance.maxFuel;
        scoreText.text = "Score: " + Mathf.FloorToInt(GameManager.Instance.score).ToString();

        int minutes = Mathf.FloorToInt(GameManager.Instance.roundTime / 60);
        int seconds = Mathf.FloorToInt(GameManager.Instance.roundTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowGameOver(string reason)
    {
        reasonText.text = reason;
        gameOverPanel.SetActive(true);
    }

    // 🏆 NEW FUNCTION
    public void ShowWinScreen()
    {
        if (winText != null)
        {
            winText.text = "CONGRATS!\nYou reached all gas stations!";
        }

        winPanel.SetActive(true);
    }
}