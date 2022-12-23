using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        public static ConnectionManager Instance { get; internal set; }
        private MainMenuManager _mainMenuManager;
        
        private string _gameSceneName = "Kolman";
        public Dictionary<ulong, bool> PlayersInGame = new Dictionary<ulong, bool>();

        private void Awake()
        {
            ManageSingleton();
            if (_mainMenuManager == null)
            {
                _mainMenuManager = FindObjectOfType<MainMenuManager>();
            }
        }
        
        private void ManageSingleton()
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

        public void StartHost(string playerName, string ipAddress, int port)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            try
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) port);
                if (NetworkManager.Singleton.StartClient())
                {
                    Debug.Log("Client started");
                }
                else
                {
                    Debug.Log("Client failed to start");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}