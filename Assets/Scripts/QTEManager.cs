// QTEManager.cs — Gas Runner
// Handles all Quick-Time Events: Changing Clothes, Metro, Gas Station, Gravity Zone.
// Attach to an empty GameObject named "QTEManager" in the level scene.

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QTEManager : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  QTE DATA                                                            //
    // ------------------------------------------------------------------ //
    [System.Serializable]
    public class QTEData
    {
        public string name;
        public string prompt;       // Text shown on screen
        public KeyCode requiredKey; // Key player must press
        public float timeWindow;    // Seconds to respond
        public float triggerX;      // Player X position that triggers this QTE
    }

    // ------------------------------------------------------------------ //
    //  INSPECTOR SETTINGS                                                  //
    // ------------------------------------------------------------------ //
    [Header("QTE Events (matches proposal)")]
    public List<QTEData> qteEvents = new List<QTEData>
    {
        new QTEData { name="Changing Clothes", prompt="Press [SPACE] to change outfit!", requiredKey=KeyCode.Space,   timeWindow=2.5f, triggerX=30f  },
        new QTEData { name="Metro Station",    prompt="Press [E] to board the train!",   requiredKey=KeyCode.E,       timeWindow=3.0f, triggerX=60f  },
        new QTEData { name="Gas Station",      prompt="Press [F] to grab the gas can!",  requiredKey=KeyCode.F,       timeWindow=2.0f, triggerX=100f },
        new QTEData { name="Gravity Zone",     prompt="Press [W] to flip gravity!",      requiredKey=KeyCode.W,       timeWindow=1.8f, triggerX=140f },
    };

    [Header("UI References")]
    public GameObject qtePanel;          // Panel that appears during QTE
    public TextMeshProUGUI promptText;    // Shows the button prompt
    public TextMeshProUGUI timerText;     // Countdown during QTE
    public TextMeshProUGUI resultText;    // "SUCCESS" or "FAILED"

    [Header("References")]
    public Transform playerTransform;

    // ------------------------------------------------------------------ //
    //  PRIVATE STATE                                                       //
    // ------------------------------------------------------------------ //
    QTEData  activeQTE;
    float    qteTimer;
    bool     qteActive;
    HashSet<float> triggeredPositions = new HashSet<float>();

    // ------------------------------------------------------------------ //
    //  UNITY LIFECYCLE                                                     //
    // ------------------------------------------------------------------ //
    void Start()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        HideQTEPanel();
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState == GameManager.GameState.GameOver) return;

        if (qteActive)
        {
            HandleActiveQTE();
        }
        else
        {
            CheckForQTETrigger();
        }
    }

    // ------------------------------------------------------------------ //
    //  TRIGGER CHECK                                                       //
    // ------------------------------------------------------------------ //
    void CheckForQTETrigger()
    {
        if (playerTransform == null) return;

        float playerX = playerTransform.position.x;

        foreach (var qte in qteEvents)
        {
            if (!triggeredPositions.Contains(qte.triggerX) && playerX >= qte.triggerX)
            {
                triggeredPositions.Add(qte.triggerX);
                StartQTE(qte);
                break;
            }
        }
    }

    // ------------------------------------------------------------------ //
    //  QTE FLOW                                                            //
    // ------------------------------------------------------------------ //
    void StartQTE(QTEData qte)
    {
        activeQTE = qte;
        qteTimer  = qte.timeWindow;
        qteActive = true;

        GameManager.Instance?.SetState(GameManager.GameState.QTE);

        ShowQTEPanel(qte.prompt);
        Debug.Log($"[QTE] Started: {qte.name} — {qte.prompt}");
    }

    void HandleActiveQTE()
    {
        // Use unscaled time because game is paused (timeScale = 0)
        qteTimer -= Time.unscaledDeltaTime;

        if (timerText != null)
            timerText.text = $"{qteTimer:F1}s";

        // Check if player pressed the correct key
        if (Input.GetKeyDown(activeQTE.requiredKey))
        {
            OnQTESuccess();
            return;
        }

        // Check for wrong key press
        if (Input.anyKeyDown)
        {
            OnQTEFail("wrong key");
            return;
        }

        // Check timeout
        if (qteTimer <= 0f)
        {
            OnQTEFail("too slow");
        }
    }

    void OnQTESuccess()
    {
        Debug.Log($"[QTE] SUCCESS — {activeQTE.name}");
        SoundManager.Instance?.PlayQTESuccess();
        StartCoroutine(ShowResultAndResume("SUCCESS!", Color.green));
        GameManager.Instance?.AddScore(100);   // Bonus for QTE success
    }

    void OnQTEFail(string reason)
    {
        Debug.Log($"[QTE] FAILED — {activeQTE.name} ({reason})");
        SoundManager.Instance?.PlayQTEFail();
        StartCoroutine(ShowResultAndResume("FAILED!", Color.red));
        GameManager.Instance?.ApplyFine($"QTE failed: {activeQTE.name}");
    }

    IEnumerator ShowResultAndResume(string message, Color color)
    {
        qteActive = false;

        if (resultText != null)
        {
            resultText.text    = message;
            resultText.color   = color;
            resultText.enabled = true;
        }

        yield return new WaitForSecondsRealtime(1.0f);

        HideQTEPanel();
        GameManager.Instance?.SetState(GameManager.GameState.Playing);
    }

    // ------------------------------------------------------------------ //
    //  UI HELPERS                                                          //
    // ------------------------------------------------------------------ //
    void ShowQTEPanel(string prompt)
    {
        if (qtePanel   != null) qtePanel.SetActive(true);
        if (promptText != null) promptText.text = prompt;
        if (resultText != null) resultText.enabled = false;
    }

    void HideQTEPanel()
    {
        if (qtePanel != null) qtePanel.SetActive(false);
    }

    public bool IsQTEActive => qteActive;
}