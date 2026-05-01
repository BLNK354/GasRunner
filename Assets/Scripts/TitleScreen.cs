using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlayTitleMusic();
    }

    public void OnStartButton()
    {
        SoundManager.Instance.PlayClick();
        SceneManager.LoadScene("GameScene");
    }

    public void OnOptionButton()
    {
        SoundManager.Instance.PlayClick();
        // shows the options panel
        OptionsManager.Instance.OpenOptions();
    }

    public void OnExitButton()
    {
        SoundManager.Instance.PlayClick();
        Application.Quit();
        Debug.Log("Game Exited");
    }

    public void OnButtonHover()
    {
        SoundManager.Instance.PlayHover();
    }
}