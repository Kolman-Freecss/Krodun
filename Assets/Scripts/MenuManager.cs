using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kolman_Freecss.Krodun
{
    public class MenuManager : MonoBehaviour
    {
        
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        
        private GameObject _mainMenu;
        private static MenuManager Instance { get; set; }
        private bool _isPaused = false;
        private GameObject _creditsCanvas;
        private bool _showCredits = false;
        private GameObject _settingsCanvas;
        private bool _showSettings = false;
        private KrodunController _krodunController;
        private GameObject _backgroundMusic;

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

            if (_backgroundMusic == null)
            {
                _backgroundMusic = GameObject.FindGameObjectWithTag("BackgroundMusic");
            }

            ManageSingleton();
        }

        private void Start()
        {
            if (_krodunController == null)
            {
                _krodunController = GetComponent<KrodunController>();
            }
        }

        public void Init()
        {
            _mainMenu.SetActive(_isPaused);
            _creditsCanvas.SetActive(_showCredits);
            _settingsCanvas.SetActive(_showSettings);
            _musicSlider.value = _backgroundMusic.GetComponent<AudioSource>().volume;
            _soundSlider.value = _krodunController.GetSoundVolume();
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
        public void OnMusicVolumeChange(float value)
        {
            this._backgroundMusic.GetComponent<AudioSource>().volume = value;
        }

        public void OnSoundVolumeChange(float volume)
        {
            this._krodunController.SetEffectsVolume(volume);
        }

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
}