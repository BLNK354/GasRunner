using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public KeyCode advanceKey = KeyCode.Return;
    public float charactersPerSecond = 45f;

    readonly Queue<DialogueLine> lines = new Queue<DialogueLine>();
    Coroutine typingRoutine;
    bool isTyping;
    string fullCurrentLine;

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2, 4)] public string text;
    }

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
        dialoguePanel?.SetActive(false);
    }

    void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(advanceKey) || Input.GetKeyDown(KeyCode.Space))
            Advance();
    }

    public void StartDialogue(List<DialogueLine> dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Count == 0) return;

        lines.Clear();
        foreach (DialogueLine line in dialogueLines)
            lines.Enqueue(line);

        dialoguePanel?.SetActive(true);
        ShowNextLine();
    }

    public void StartSingleLine(string speaker, string text)
    {
        StartDialogue(new List<DialogueLine>
        {
            new DialogueLine { speaker = speaker, text = text }
        });
    }

    public void Advance()
    {
        if (isTyping)
        {
            FinishTyping();
            return;
        }

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = lines.Dequeue();
        if (speakerText != null)
            speakerText.text = line.speaker;

        fullCurrentLine = line.text;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine(fullCurrentLine));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.text = "";

        float delay = charactersPerSecond > 0f ? 1f / charactersPerSecond : 0f;
        foreach (char character in line)
        {
            if (dialogueText != null)
                dialogueText.text += character;

            if (delay > 0f)
                yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;
    }

    void FinishTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (dialogueText != null)
            dialogueText.text = fullCurrentLine;

        isTyping = false;
    }

    void EndDialogue()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        isTyping = false;
        lines.Clear();
        dialoguePanel?.SetActive(false);
    }
}
