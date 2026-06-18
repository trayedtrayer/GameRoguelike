using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public static bool isPaused = false;

    [Header("Root")]
    public GameObject background;

    [Header("Windows")]
    public GameObject mainWindow;
    public GameObject settingsWindow;
    public GameObject controlsWindow;

    [Header("Audio")]
    public Slider sfxSlider;
    public AudioMixer audioMixer;

    GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        background.SetActive(false);
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(savedVolume) * 20);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplySettings();
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (!IsGameplayScene())
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) CloseMenu();
            else OpenMenu();
        }
    }

    private bool IsGameplayScene()
    {
        return SceneManager.GetActiveScene().buildIndex != 0;
    }

    public void OpenMenu()
    {
        background.SetActive(true);
        ShowMain();

        if(player == null)
        {
            GameObject.Find("Player");
        }

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        background.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
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
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseAndBlockPlayerWindows()
    {
        player.GetComponent<UpgradeUI>().CloseMenuAndBlock();
        player.GetComponent<Crafting>().CloseAndBlock();
        player.GetComponent<ArsenalChest>().CloseAndBlock();
    }

    public void UnlockWindows()
    {
        player.GetComponent<UpgradeUI>().Unlock();
        player.GetComponent<Crafting>().Unlock();
        player.GetComponent<ArsenalChest>().Unlock();
    }
}