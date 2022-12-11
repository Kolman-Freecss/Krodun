using System;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kolman_Freecss.Krodun
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [HideInInspector]
        public NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);
        
        [HideInInspector]
        public delegate void IsSceneLoadedDelegateHandler(bool isLoaded);
        
        [HideInInspector]
        public event IsSceneLoadedDelegateHandler OnSceneLoadedChanged;

        [HideInInspector] public bool isSceneLoadedValue;
        
        internal static event Action OnSingletonReady;
    
        private void Awake()
        {
            Assert.IsNull(Instance, $"Multiple instances of {nameof(Instance)} detected. This should not happen.");
            Instance = this;
            DontDestroyOnLoad(this);
            
            OnSingletonReady?.Invoke();
            if (IsServer)
            {
                isGameStarted.Value = false;
            }
        }
        
        public override void OnNetworkSpawn()
        {
            /*if (IsClient && IsServer)
            }*/
            
            if (IsServer)
            {
                //Server will be notified when a client connects
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene += ClientLoadedScene;
            }
            
        }
        
        private void ClientLoadedScene(ulong clientId)
        {
            if (IsServer)
            {
                OnClientConnectedCallbackClientRpc(clientId);
            }
        }
        
        [ClientRpc]
        private void OnClientConnectedCallbackClientRpc(ulong clientId)
        {
            Debug.Log("------------------SEND Client Loaded Scene------------------");
            OnSceneLoadedChanged?.Invoke(true);
            isSceneLoadedValue = true;
            Debug.Log("IsSceneLoaded -> " + GameManager.Instance.isSceneLoadedValue);
            StartGame();
        }

        public void Update()
        {
            if (IsServer)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            if (!isGameStarted.Value && SceneTransitionHandler.sceneTransitionHandler.GetCurrentSceneState() == SceneTransitionHandler.SceneStates.Kolman)
            {
                Debug.Log("------------------START GAME------------------");
                isGameStarted.Value = true;
            }
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"Client {clientId} connected");
            if (IsServer)
            {
                if (!ConnectionManager.Instance.PlayersInGame.ContainsKey(clientId))
                {
                    ConnectionManager.Instance.PlayersInGame.Add(clientId, false);
                }
            }
        }
    }
}