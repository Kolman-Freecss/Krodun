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
        
        internal static event Action OnSingletonReady;
    
        private void Awake()
        {
            Assert.IsNull(Instance, $"Multiple instances of {nameof(Instance)} detected. This should not happen.");
            Instance = this;
            
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
            if (!isGameStarted.Value)
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