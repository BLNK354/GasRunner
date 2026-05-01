using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load saved settings
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyAudioSettings();

        // Start on Audio tab
        ShowAudioPanel();
    }

    // -------------------------------------------------------
    // OPEN / CLOSE
    // -------------------------------------------------------

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        ShowAudioPanel();
    }

    public void CloseOptions()
    {
        SoundManager.Instance.PlayClick();
        optionsPanel.SetActive(false);
    }

    // -------------------------------------------------------
    // TAB SWITCHING
    // -------------------------------------------------------

    public void ShowAudioPanel()
    {
        SoundManager.Instance.PlayClick();
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void ShowControlsPanel()
    {
        SoundManager.Instance.PlayClick();
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // -------------------------------------------------------
    // AUDIO SETTINGS
    // -------------------------------------------------------

    public void OnMasterVolumeChanged()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        AudioListener.volume = masterVolumeSlider.value;
    }

    public void OnMusicVolumeChanged()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        SoundManager.Instance.SetMusicVolume(musicVolumeSlider.value);
    }

    public void OnSFXVolumeChanged()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }

    void ApplyAudioSettings()
    {
        AudioListener.volume = masterVolumeSlider.value;
        SoundManager.Instance.SetMusicVolume(musicVolumeSlider.value);
    }
}