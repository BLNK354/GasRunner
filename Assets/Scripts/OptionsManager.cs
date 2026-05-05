using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Gameplay Sliders")]
    public Slider playerSpeedSlider;
    public Slider jumpForceSlider;
    public Slider obstacleRateSlider;
    public TextMeshProUGUI playerSpeedValueText;
    public TextMeshProUGUI jumpForceValueText;
    public TextMeshProUGUI obstacleRateValueText;

    [Header("Controls")]
    public Button jumpKeyButton;
    public Button pauseKeyButton;
    public TextMeshProUGUI jumpKeyText;
    public TextMeshProUGUI pauseKeyText;
    public TextMeshProUGUI rebindPromptText;

    string pendingRebindPlayerPrefsKey;
    TextMeshProUGUI pendingRebindLabel;
    KeyCode[] keyCodes;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
    }

    void Start()
    {
        SetSliderValue(masterVolumeSlider, GameSettings.MasterVolume);
        SetSliderValue(musicVolumeSlider, GameSettings.MusicVolume);
        SetSliderValue(sfxVolumeSlider, GameSettings.SfxVolume);
        SetSliderValue(playerSpeedSlider, GameSettings.PlayerSpeedMultiplier);
        SetSliderValue(jumpForceSlider, GameSettings.JumpForceMultiplier);
        SetSliderValue(obstacleRateSlider, GameSettings.ObstacleRateMultiplier);

        ApplyAudioSettings();
        RefreshGameplayLabels();
        RefreshControlLabels();
        HookControlButtons();

        // Start on Audio tab
        ShowAudioPanel();
    }

    void Update()
    {
        if (string.IsNullOrEmpty(pendingRebindPlayerPrefsKey)) return;

        foreach (KeyCode keyCode in keyCodes)
        {
            if (!Input.GetKeyDown(keyCode)) continue;

            GameSettings.SaveKeyCode(pendingRebindPlayerPrefsKey, keyCode);
            pendingRebindPlayerPrefsKey = null;
            pendingRebindLabel = null;
            RefreshControlLabels();
            SetRebindPrompt("");
            SoundManager.Instance?.PlayClick();
            break;
        }
    }

    // -------------------------------------------------------
    // OPEN / CLOSE
    // -------------------------------------------------------

    public void OpenOptions()
    {
        optionsPanel?.SetActive(true);
        ShowAudioPanel();
    }

    public void CloseOptions()
    {
        SoundManager.Instance?.PlayClick();
        optionsPanel?.SetActive(false);
        CancelRebind();
    }

    // -------------------------------------------------------
    // TAB SWITCHING
    // -------------------------------------------------------

    public void ShowAudioPanel()
    {
        SoundManager.Instance?.PlayClick();
        audioPanel?.SetActive(true);
        controlsPanel?.SetActive(false);
        CancelRebind();
    }

    public void ShowControlsPanel()
    {
        SoundManager.Instance?.PlayClick();
        audioPanel?.SetActive(false);
        controlsPanel?.SetActive(true);
    }

    // -------------------------------------------------------
    // AUDIO SETTINGS
    // -------------------------------------------------------

    public void OnMasterVolumeChanged()
    {
        if (masterVolumeSlider == null) return;

        GameSettings.SaveFloat(GameSettings.MasterVolumeKey, masterVolumeSlider.value);
        AudioListener.volume = masterVolumeSlider.value;
    }

    public void OnMusicVolumeChanged()
    {
        if (musicVolumeSlider == null) return;

        GameSettings.SaveFloat(GameSettings.MusicVolumeKey, musicVolumeSlider.value);
        SoundManager.Instance?.SetMusicVolume(musicVolumeSlider.value);
    }

    public void OnSFXVolumeChanged()
    {
        if (sfxVolumeSlider == null) return;

        GameSettings.SaveFloat(GameSettings.SfxVolumeKey, sfxVolumeSlider.value);
        SoundManager.Instance?.SetSFXVolume(sfxVolumeSlider.value);
    }

    void ApplyAudioSettings()
    {
        AudioListener.volume = masterVolumeSlider != null ? masterVolumeSlider.value : GameSettings.MasterVolume;
        SoundManager.Instance?.SetMusicVolume(musicVolumeSlider != null ? musicVolumeSlider.value : GameSettings.MusicVolume);
        SoundManager.Instance?.SetSFXVolume(sfxVolumeSlider != null ? sfxVolumeSlider.value : GameSettings.SfxVolume);
    }

    // -------------------------------------------------------
    // GAMEPLAY SETTINGS
    // -------------------------------------------------------

    public void OnPlayerSpeedChanged()
    {
        if (playerSpeedSlider == null) return;

        GameSettings.SaveFloat(GameSettings.PlayerSpeedKey, playerSpeedSlider.value);
        RefreshGameplayLabels();
    }

    public void OnJumpForceChanged()
    {
        if (jumpForceSlider == null) return;

        GameSettings.SaveFloat(GameSettings.JumpForceKey, jumpForceSlider.value);
        RefreshGameplayLabels();
    }

    public void OnObstacleRateChanged()
    {
        if (obstacleRateSlider == null) return;

        GameSettings.SaveFloat(GameSettings.ObstacleRateKey, obstacleRateSlider.value);
        RefreshGameplayLabels();
    }

    public void ResetGameplaySettings()
    {
        GameSettings.ResetGameplay();
        SetSliderValue(playerSpeedSlider, GameSettings.PlayerSpeedMultiplier);
        SetSliderValue(jumpForceSlider, GameSettings.JumpForceMultiplier);
        SetSliderValue(obstacleRateSlider, GameSettings.ObstacleRateMultiplier);
        RefreshGameplayLabels();
    }

    void RefreshGameplayLabels()
    {
        SetValueText(playerSpeedValueText, playerSpeedSlider);
        SetValueText(jumpForceValueText, jumpForceSlider);
        SetValueText(obstacleRateValueText, obstacleRateSlider);
    }

    // -------------------------------------------------------
    // CONTROL REBINDING
    // -------------------------------------------------------

    public void StartJumpRebind()
    {
        BeginRebind(GameSettings.JumpControlKey, jumpKeyText, "Press a key for Jump");
    }

    public void StartPauseRebind()
    {
        BeginRebind(GameSettings.PauseControlKey, pauseKeyText, "Press a key for Pause");
    }

    public void ResetControls()
    {
        GameSettings.ResetControls();
        RefreshControlLabels();
        CancelRebind();
    }

    void BeginRebind(string playerPrefsKey, TextMeshProUGUI label, string prompt)
    {
        pendingRebindPlayerPrefsKey = playerPrefsKey;
        pendingRebindLabel = label;
        SetLabelText(pendingRebindLabel, "...");
        SetRebindPrompt(prompt);
        SoundManager.Instance?.PlayClick();
    }

    void CancelRebind()
    {
        if (string.IsNullOrEmpty(pendingRebindPlayerPrefsKey)) return;

        pendingRebindPlayerPrefsKey = null;
        pendingRebindLabel = null;
        RefreshControlLabels();
        SetRebindPrompt("");
    }

    void RefreshControlLabels()
    {
        SetLabelText(jumpKeyText, GameSettings.JumpKey.ToString());
        SetLabelText(pauseKeyText, GameSettings.PauseKey.ToString());
    }

    void HookControlButtons()
    {
        jumpKeyButton?.onClick.AddListener(StartJumpRebind);
        pauseKeyButton?.onClick.AddListener(StartPauseRebind);
    }

    void SetRebindPrompt(string message)
    {
        if (rebindPromptText != null)
            rebindPromptText.text = message;
    }

    static void SetSliderValue(Slider slider, float value)
    {
        if (slider != null)
            slider.value = value;
    }

    static void SetValueText(TextMeshProUGUI label, Slider slider)
    {
        if (label != null && slider != null)
            label.text = $"{slider.value:0.00}x";
    }

    static void SetLabelText(TextMeshProUGUI label, string value)
    {
        if (label != null)
            label.text = value;
    }
}
