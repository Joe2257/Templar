using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameManager gameManager;
    public Shared      sharedVar;
    [Header("Menus")]
    public GameObject _mainMenu;
    public GameObject _settingsWindow;
    public GameObject _credits;

    [Header("Settings")]
    [SerializeField] private Dropdown _resolutionDropDown = null;
    [SerializeField] private Slider   _volumeSlider       = null;
    [SerializeField] private Dropdown _qualityDropDown    = null;


    private Resolution[] _resolutions;

    private List<string> _resolutionOptions = new List<string>();

    public AudioMixer audioMixer;

    void Start()
    {
        InitializeSettingsOptions();
    }


    void Update()
    {
        
    }

    public void OnStartButton()
    {
        sharedVar.loadPlayer = false;

        Time.timeScale = 1;

        gameManager.LoadSceneByString("Village");
    }

    public void OnLoadButton()
    {
        sharedVar.loadPlayer = true;

        DataToSave savedData = SaveSystem.LoadPlayer();

        gameManager.LoadSceneByString(savedData.levelToLoad);

        Time.timeScale = 1;
    }

    public void OnSettingsButton()
    {
        _mainMenu.SetActive(false);
        _settingsWindow.SetActive(true);
        _credits.SetActive(false);
    }

    public void OnCreditsButton()
    {
        _mainMenu.SetActive(false);
        _settingsWindow.SetActive(false);
        _credits.SetActive(true);
    }

    public void OnSaveAndReturnButton()
    {
        _mainMenu.SetActive(true);
        _settingsWindow.SetActive(false);
        _credits.SetActive(false);

        SaveSettings();
    }

    public void OnReturnButton()
    {
        _mainMenu.SetActive(true);
        _settingsWindow.SetActive(false);
        _credits.SetActive(false);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    //____________Settings_____________\\

    private void InitializeSettingsOptions()
    {
        //SetVolume
        if (!PlayerPrefs.HasKey("Volume"))
        {
            _volumeSlider.value = 0f;
            audioMixer.SetFloat("MasterVolume", _volumeSlider.value);
        }
        else
        {
            _volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            audioMixer.SetFloat("MasterVolume", _volumeSlider.value);
        }

        //SetQuality
        if (!PlayerPrefs.HasKey("Quality"))
        {
            QualitySettings.SetQualityLevel(2);

            if (_settingsWindow.activeSelf)
                _qualityDropDown.value = QualitySettings.GetQualityLevel();
        }
        else
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
            _qualityDropDown.value = QualitySettings.GetQualityLevel();
        }


        //SetResolution
        if (!PlayerPrefs.HasKey("Resolution"))
        {
            _resolutions = Screen.resolutions;
            _resolutionDropDown.ClearOptions();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;

                _resolutionOptions.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            _resolutionDropDown.AddOptions(_resolutionOptions);
            _resolutionDropDown.value = currentResolutionIndex;
            _resolutionDropDown.RefreshShownValue();
        }
        else
        {
            _resolutions = Screen.resolutions;
            _resolutionDropDown.ClearOptions();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;

                _resolutionOptions.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            _resolutionDropDown.AddOptions(_resolutionOptions);
            _resolutionDropDown.value = PlayerPrefs.GetInt("Resolution");
            _resolutionDropDown.RefreshShownValue();
        }

    }

    //____________SettingsFunctions_____________\\

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    public void SetVolumeFromSlider(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetQualityIndex(int qualityIndex)
    {
        qualityIndex = _qualityDropDown.value;

        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", _volumeSlider.value);
        PlayerPrefs.SetInt("Quality", _qualityDropDown.value);
        PlayerPrefs.SetInt("Resolution", _resolutionDropDown.value);

    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        InitializeSettingsOptions();
    }
}
