using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun
{
    public class MainMenuManager : MonoBehaviour
    {
        private GameObject _mainMenuManager;
        private static MainMenuManager Instance { get; set; }
        private GameObject _multiplayerCanvas;

        private void Awake()
        {
            if (_mainMenuManager == null)
            {
                _mainMenuManager = GameObject.FindGameObjectWithTag("MainMenuManager");
            }
            
            if (_multiplayerCanvas == null)
            {
                _multiplayerCanvas = GameObject.FindGameObjectWithTag("MultiplayerCanvas");
            }

            ManageSingleton();
        }

        private void Start()
        {
            _multiplayerCanvas.SetActive(false);
        }

        public void OnApplicationQuit()
        {
            Application.Quit();
        }
        
        public void OnMultiplayerPressed()
        {
            _multiplayerCanvas.SetActive(true);
        }
        
        public void OnStartButtonPressed()
        {
            StartCoroutine(LoadGame());
        }
        
        IEnumerator LoadGame()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Kolman");
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
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
    }
}