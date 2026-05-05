using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Music")]
    public AudioClip bgMusic;

    [Header("SFX")]
    public AudioClip qteSucess;
    public AudioClip qteFail;
    public AudioClip obstacleHit;
    public AudioClip fuelPickup;
    public AudioClip timeWarning;
    public AudioClip click;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // Singleton - only one SoundManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        sfxSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        SetMusicVolume(GameSettings.MusicVolume);
        SetSFXVolume(GameSettings.SfxVolume);
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (bgMusic == null || musicSource == null) return;

        musicSource.clip = bgMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayQTESuccess() => PlaySFX(qteSucess);
    public void PlayQTEFail() => PlaySFX(qteFail);
    public void PlayObstacleHit() => PlaySFX(obstacleHit);
    public void PlayFuelPickup() => PlaySFX(fuelPickup);
    public void PlayTimeWarning() => PlaySFX(timeWarning);
    public void PlayClick() => PlaySFX(click);

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }

    public void SetMusicPitch(float pitch)
    {
        if (musicSource == null) return;

        musicSource.pitch = pitch; // Speed up music as timer gets low
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}
