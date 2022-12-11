using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun
{
    public class MainMenuManager : MonoBehaviour
    {
        private GameObject _multiplayerCanvas;

        private void Awake()
        {
            if (_multiplayerCanvas == null)
            {
                _multiplayerCanvas = GameObject.FindGameObjectWithTag("MultiplayerCanvas");
            }
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
            StartGame();
        }
        
        public void StartGame()
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

            yield return new WaitForEndOfFrame();
        }
    }
}