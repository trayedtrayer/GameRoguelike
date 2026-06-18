using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [Header("Windows")]
    public GameObject mainWindow;
    public GameObject settingsWindow;
    public GameObject controlsWindow;

    [Header("Audio")]
    public Slider sfxSlider;
    public AudioMixer audioMixer;

    private void Start()
    {
        ShowMain();
        Cursor.visible = true;
        Time.timeScale = 1f;

        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        SetSFXVolume(savedVolume);
    }

    public void PlayGame()
    {
        print(1);
        SceneManager.LoadScene(1);
    }

    public void ShowMain()
    {
        mainWindow.SetActive(true);
        settingsWindow.SetActive(false);
        controlsWindow.SetActive(false);
    }

    public void ShowSettings()
    {
        mainWindow.SetActive(false);
        settingsWindow.SetActive(true);
        controlsWindow.SetActive(false);
    }

    public void ShowControls()
    {
        mainWindow.SetActive(false);
        settingsWindow.SetActive(false);
        controlsWindow.SetActive(true);
    }

    public void ApplySettings()
    {
        float value = sfxSlider.value;

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        SetSFXVolume(value);
    }

    private void SetSFXVolume(float value)
    {
        if (value < 0.0001f)
            value = 0.0001f;

        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}