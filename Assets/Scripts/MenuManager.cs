using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameObject _mainMenu;
    private static MenuManager Instance;
    private bool _isPaused = false;
    private GameObject _creditsCanvas;
    private bool _showCredits = false;
    private GameObject _settingsCanvas;
    private bool _showSettings = false;

    private void Awake()
    {
        if (_mainMenu == null)
        {
            _mainMenu = GameObject.FindGameObjectWithTag("Menu");
        }

        if (_creditsCanvas == null)
        {
            _creditsCanvas = GameObject.FindGameObjectWithTag("Credits");
        }
        
        if (_settingsCanvas == null)
        {
            _settingsCanvas = GameObject.FindGameObjectWithTag("Settings");
        }

        ManageSingleton();
    }

    public void Init()
    {
        _mainMenu.SetActive(_isPaused);
        _creditsCanvas.SetActive(_showCredits);
        _settingsCanvas.SetActive(_showSettings);
    }

    void ManageSingleton()
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        CloseMenu();
        CloseCredits();
        CloseSettings();
    }

    /**################## MENU ##################**/
    public void ToggleMenu()
    {
        _isPaused = !_isPaused;
        _mainMenu.SetActive(_isPaused);
        CloseCredits();
        CloseSettings();
    }

    public void CloseMenu()
    {
        _isPaused = false;
        _mainMenu.SetActive(_isPaused);
    }
    
    /**################## CREDITS ##################**/
    public void ToggleCredits()
    {
        _showCredits = !_showCredits;
        _creditsCanvas.SetActive(_showCredits);
        if (_showCredits) CloseMenu();
        else ToggleMenu();
    }

    private void CloseCredits()
    {
        _showCredits = false;
        _creditsCanvas.SetActive(_showCredits);
    }
    
    /**################## SETTINGS ##################**/
    public void ToggleSettings()
    {
        _showSettings = !_showSettings;
        _settingsCanvas.SetActive(_showSettings);
        if (_showSettings) CloseMenu();
        else ToggleMenu();
    }

    private void CloseSettings()
    {
        _showSettings = false;
        _settingsCanvas.SetActive(_showSettings);
    }
}