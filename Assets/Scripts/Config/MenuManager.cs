using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Kolman_Freecss.Krodun
{
    public class MenuManager : MonoBehaviour
    {
        
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        
        private GameObject _mainMenu;
        private bool _isPaused = false;
        private GameObject _creditsCanvas;
        private bool _showCredits = false;
        private GameObject _settingsCanvas;
        private bool _showSettings = false;
        private KrodunController _krodunController;

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

        }

        private void Start()
        {
            if (_krodunController == null)
            {
                _krodunController = FindObjectsOfType<KrodunController>().FirstOrDefault(k => k.OwnerClientId == NetworkManager.Singleton.LocalClientId);
            }

            _musicSlider.value = SoundManager.Instance.MusicAudioVolume;
            _soundSlider.value = SoundManager.Instance.EffectsAudioVolume;
        }

        public void Init()
        {
            _mainMenu.SetActive(_isPaused);
            _creditsCanvas.SetActive(_showCredits);
            _settingsCanvas.SetActive(_showSettings);
        }

        public void ExitGame()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
            Application.Quit();
        }

        public void ContinueGame()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
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
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
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
            SoundManager.Instance.SetMusicVolume(this._musicSlider.value);
        }

        public void OnSoundVolumeChange(float volume)
        {
            SoundManager.Instance.SetEffectsVolume(this._soundSlider.value);
        }

        public void ToggleSettings()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
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