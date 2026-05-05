using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GasFillMinigame : MonoBehaviour
{
    public static GasFillMinigame Instance { get; private set; }

    [Header("UI")]
    public GameObject minigamePanel;
    public Slider fillSlider;
    public Image fillImage;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    [Header("Rules")]
    public KeyCode fillKey = KeyCode.F;
    public float targetFill = 1f;
    public float fillPerPress = 0.08f;
    public float drainPerSecond = 0.12f;
    public float timeLimit = 6f;
    public float fuelReward = 100f;
    public int scoreReward = 250;

    float currentFill;
    float timeRemaining;
    bool isActive;
    GasStation activeStation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (!isActive) return;

        timeRemaining -= Time.unscaledDeltaTime;
        currentFill = Mathf.Max(0f, currentFill - drainPerSecond * Time.unscaledDeltaTime);

        if (Input.GetKeyDown(fillKey))
        {
            currentFill = Mathf.Min(targetFill, currentFill + fillPerPress);
            SoundManager.Instance?.PlayFuelPickup();
        }

        UpdateUI();

        if (currentFill >= targetFill)
        {
            CompleteMinigame();
            return;
        }

        if (timeRemaining <= 0f)
            FailMinigame();
    }

    public void StartMinigame(GasStation station)
    {
        if (isActive) return;

        activeStation = station;
        currentFill = 0f;
        timeRemaining = timeLimit;
        isActive = true;

        GameManager.Instance?.SetState(GameManager.GameState.QTE);
        minigamePanel?.SetActive(true);

        if (resultText != null)
            resultText.text = "";

        UpdateUI();
    }

    void CompleteMinigame()
    {
        isActive = false;
        FuelSystem.Instance?.Refill(fuelReward);
        GameManager.Instance?.AddScore(scoreReward);

        if (resultText != null)
            resultText.text = "FULL!";

        Hide();
        activeStation?.CompleteStation();
        GameManager.Instance?.SetState(GameManager.GameState.Playing);
    }

    void FailMinigame()
    {
        isActive = false;

        if (resultText != null)
            resultText.text = "FAILED";

        Hide();
        GameManager.Instance?.ApplyFine("gas fill failed");
        GameManager.Instance?.SetState(GameManager.GameState.Playing);
        activeStation?.ResetStation();
    }

    void UpdateUI()
    {
        float normalizedFill = targetFill > 0f ? currentFill / targetFill : 0f;

        if (fillSlider != null)
            fillSlider.value = normalizedFill;

        if (fillImage != null)
            fillImage.fillAmount = normalizedFill;

        if (promptText != null)
            promptText.text = $"Press [{fillKey}] to fill gas";

        if (timerText != null)
            timerText.text = $"{Mathf.Max(0f, timeRemaining):0.0}s";
    }

    void Hide()
    {
        minigamePanel?.SetActive(false);
    }
}
