using System.Collections.Generic;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Model;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Kolman_Freecss.Krodun
{
    public class MultiplayerLobbyManager : NetworkBehaviour
    {
        [SerializeField] 
        private GameObject playersInLobbyWrapper;
        [SerializeField]
        private GameObject playerInLobbyPrefab;
        [SerializeField]
        private Button startGameButton;
        private string _gameSceneName = "Kolman";
        private string _mainMenuScene = "MainMenu";

        private int minPlayers = 2;
        private int maxPlayers = 2;
        
        private Dictionary<ulong, GameObject> _playersReady = new Dictionary<ulong, GameObject>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                startGameButton.GetComponent<CanvasGroup>().alpha = 1;
                startGameButton.GetComponent<CanvasGroup>().interactable = true;
                startGameButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
                OnClientConnected(NetworkManager.Singleton.LocalClientId);
            }

        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        private void OnClientDisconnect(ulong obj)
        {
            if (_playersReady.ContainsKey(obj))
            {
                Destroy(_playersReady[obj]);
                _playersReady[obj].GetComponent<NetworkObject>().Despawn();
                _playersReady.Remove(obj);
                ConnectionManager.Instance.RemovePlayer(obj);
            }
        }

        private void OnClientConnected(ulong obj)
        {
            Player player = ConnectionManager.Instance.PlayersInGame[obj];
            GenerateUserForLobby(player);
        }
        
        private void GenerateUserForLobby(Player player)
        {
            GameObject playerLobby = Instantiate(playerInLobbyPrefab, playersInLobbyWrapper.transform);
            // Get the Name child of the playerLobby
            TextMeshProUGUI playerName = playerLobby.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerName.text = player.Name;
            // Get the State child of the playerLobby
            TextMeshProUGUI playerReady = playerLobby.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            playerReady.text = "Not Ready";
            //playerLobby.GetComponent<NetworkObject>().Spawn();
            _playersReady.Add(player.Id, playerLobby);
            CheckAllPlayersReady();
        }
        
        private void UpdateUsersForLobby(ulong clientId)
        {
            GameObject playerLobby = _playersReady[clientId];
            // Get the State child of the playerLobby
            TextMeshProUGUI playerReady = playerLobby.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Player player = ConnectionManager.Instance.PlayersInGame[clientId];
            player.IsReady = !player.IsReady;
            playerReady.text = player.IsReady ? "Ready" : "Not Ready";
        }

        public void OnReadyButton()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
            OnClientIsReadyServerRpc();
        }

        public void OnStartGame()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
            if (!CheckAllPlayersReady()) return;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            SceneTransitionHandler.sceneTransitionHandler.RegisterGameCallbacks();
            //Transition to the ingame scene
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_gameSceneName);
        }

        private bool CheckAllPlayersReady()
        {
            if (ConnectionManager.Instance.PlayersInGame.Count >= minPlayers)
            {
                foreach (KeyValuePair<ulong, Player> player in ConnectionManager.Instance.PlayersInGame)
                {
                    if (!player.Value.IsReady)
                    {
                        DisableStartButton();
                        return false;
                    }
                }
                return true;
            }
            DisableStartButton();
            return false;
        }

        public void DisableStartButton()
        {
            startGameButton.interactable = false;
            startGameButton. GetComponent<CanvasGroup>().alpha = 0.5f;
            startGameButton. GetComponent<CanvasGroup>().interactable = false;
            startGameButton. GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnClientIsReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            if (_playersReady.ContainsKey(clientId))
            {
                CheckAllPlayersReady();
                UpdateUsersForLobby(clientId);
            }
        }
        
        public void OnClose()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                // Close the server
                NetworkManager.Singleton.Shutdown();
            }
            else
            {
                // Disconnect from the server
                ClientDisconnectServerRpc();
            }
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_mainMenuScene);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ClientDisconnectServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            if (_playersReady.ContainsKey(clientId))
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
            }
        }
    }
}