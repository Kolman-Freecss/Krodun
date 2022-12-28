using System;
using Model;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager Instance { get; internal set; }
        private MainMenuManager _mainMenuManager;
        
        private string _gameSceneName = "Kolman";
        private string _lobbySceneName = "MultiplayerLobby";

        public NetworkList<Player> PlayersInGame;
        public string PlayerName;

        private void Awake()
        {
            ManageSingleton();
            if (_mainMenuManager == null)
            {
                _mainMenuManager = FindObjectOfType<MainMenuManager>();
            }
            PlayersInGame  = new NetworkList<Player>(default, NetworkVariableReadPermission.Everyone ,NetworkVariableWritePermission.Owner );
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
                PlayerName = playerName;
                if (NetworkManager.Singleton.StartHost())
                {
                    // SceneTransitionHandler.sceneTransitionHandler.RegisterGameCallbacks();
                    SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_lobbySceneName);
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
                PlayerName = playerName;
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

        [ServerRpc]
        public void DisconnectGameServerRpc()
        {
            PlayersInGame.Clear();
        }
    }
}