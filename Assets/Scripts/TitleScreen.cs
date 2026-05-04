using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TitleScreen.cs — Gas Runner Main Menu
/// Handles title screen UI interactions and scene transitions.
/// Attach to an empty GameObject in the MainMenu scene.
/// </summary>
public class TitleScreen : MonoBehaviour
{
    void Start()
    {
        // Reset game state when returning to menu
        Time.timeScale = 1f;
        
        // Play menu music
        SoundManager.Instance?.PlayMusic();
        
        Debug.Log("[TitleScreen] Main menu loaded.");
    }

    /// <summary>Start a new game through GameManager.</summary>
    public void PlayGame()
    {
        Debug.Log("[TitleScreen] Starting game...");
        GameManager.Instance?.StartGame();
    }

    /// <summary>Load settings screen.</summary>
    public void OpenSettings()
    {
        Debug.Log("[TitleScreen] Opening settings...");
        SceneManager.LoadScene("Settings");
    }

    /// <summary>Exit application.</summary>
    public void QuitGame()
    {
        Debug.Log("[TitleScreen] Quitting game.");
        Application.Quit();
    }
}