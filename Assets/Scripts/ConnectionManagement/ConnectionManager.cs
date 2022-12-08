using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        private MainMenuManager _mainMenuManager;
        
        private string _gameSceneName = "Kolman";
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_mainMenuManager == null)
            {
                _mainMenuManager = FindObjectOfType<MainMenuManager>();
            }
        }

        public void StartHost(string playerName, string ipAddress, int port)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) port);
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
        
        private void LoadGame()
        {
            NetworkManager.Singleton.SceneManager.OnSynchronize += SceneManager_LoadGame; 
        }

        private void SceneManager_LoadGame(ulong clientId)
        {
            _mainMenuManager.StartGame();
        }
        
        public void StartClient(string playerName, string ipAddress, int port)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) port);
            //StartGame();
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client started");
            }
            else
            {
                Debug.Log("Client failed to start");
            }
        }

        public void StartGame()
        {
            _mainMenuManager.StartGame();
        }

    }
}