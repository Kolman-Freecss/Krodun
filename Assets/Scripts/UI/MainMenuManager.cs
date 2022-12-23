using System;
using System.Collections;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun
{
    public class MainMenuManager : MonoBehaviour
    {
        private GameObject _multiplayerCanvas;
        private float _delayTimeToStart = 3f; 
        private string _gameSceneName = "Kolman";

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
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            Application.Quit();
        }

        public void OnMultiplayerPressed()
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            _multiplayerCanvas.SetActive(true);
        }

        public void OnStartButtonPressed()
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            StartGame();
        }
        
        public void StartGame()
        {
            StartCoroutine(LoadGame());
        }

        IEnumerator LoadGame()
        {
            yield return new WaitForSeconds(_delayTimeToStart);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", (ushort) 6666);
            if (NetworkManager.Singleton.StartHost())
            {
                SceneTransitionHandler.sceneTransitionHandler.RegisterCallbacks();
                SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_gameSceneName);
                Debug.Log("Host started");
            }
            else
            {
                Debug.Log("Host failed to start");
            }
            
        }
    }
}