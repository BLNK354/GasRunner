using System;
using UnityEngine;

public static class GameSettings
{
    public const string MasterVolumeKey = "MasterVolume";
    public const string MusicVolumeKey = "MusicVolume";
    public const string SfxVolumeKey = "SFXVolume";
    public const string PlayerSpeedKey = "PlayerSpeedMultiplier";
    public const string JumpForceKey = "JumpForceMultiplier";
    public const string ObstacleRateKey = "ObstacleRateMultiplier";
    public const string JumpControlKey = "JumpKey";
    public const string PauseControlKey = "PauseKey";

    public static float MasterVolume => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
    public static float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 0.6f);
    public static float SfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    public static float PlayerSpeedMultiplier => PlayerPrefs.GetFloat(PlayerSpeedKey, 1f);
    public static float JumpForceMultiplier => PlayerPrefs.GetFloat(JumpForceKey, 1f);
    public static float ObstacleRateMultiplier => PlayerPrefs.GetFloat(ObstacleRateKey, 1f);
    public static KeyCode JumpKey => GetKeyCode(JumpControlKey, KeyCode.Space);
    public static KeyCode PauseKey => GetKeyCode(PauseControlKey, KeyCode.Escape);

    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static void SaveKeyCode(string key, KeyCode value)
    {
        PlayerPrefs.SetString(key, value.ToString());
        PlayerPrefs.Save();
    }

    public static void ResetGameplay()
    {
        PlayerPrefs.DeleteKey(PlayerSpeedKey);
        PlayerPrefs.DeleteKey(JumpForceKey);
        PlayerPrefs.DeleteKey(ObstacleRateKey);
        PlayerPrefs.Save();
    }

    public static void ResetControls()
    {
        PlayerPrefs.DeleteKey(JumpControlKey);
        PlayerPrefs.DeleteKey(PauseControlKey);
        PlayerPrefs.Save();
    }

    static KeyCode GetKeyCode(string key, KeyCode fallback)
    {
        string savedValue = PlayerPrefs.GetString(key, fallback.ToString());
        return Enum.TryParse(savedValue, out KeyCode parsedValue) ? parsedValue : fallback;
    }
}
