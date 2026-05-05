using UnityEngine;

public class GasStation : MonoBehaviour
{
    [Header("Minigame")]
    public GasFillMinigame gasFillMinigame;
    public bool requireMinigame = true;

    [Header("Dialogue")]
    public string attendantName = "Gas Station";
    [TextArea(2, 3)] public string arrivalDialogue = "Fill the tank before time runs out.";

    bool activated;

    void Start()
    {
        if (gasFillMinigame == null)
            gasFillMinigame = GasFillMinigame.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        TryActivate(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryActivate(other.gameObject);
    }

    void TryActivate(GameObject other)
    {
        if (activated || !other.CompareTag("Player")) return;

        activated = true;
        DialogueManager.Instance?.StartSingleLine(attendantName, arrivalDialogue);

        if (requireMinigame && gasFillMinigame != null)
        {
            gasFillMinigame.StartMinigame(this);
            Debug.Log("[GasStation] Gas fill minigame started.");
            return;
        }

        CompleteStation();
    }

    public void CompleteStation()
    {
        Debug.Log("[GasStation] Gas station completed.");
        GameManager.Instance?.ReachStation();
    }

    public void ResetStation()
    {
        activated = false;
    }
}
