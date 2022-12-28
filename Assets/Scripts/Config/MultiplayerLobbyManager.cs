using System.Collections.Generic;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Model;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private readonly string _gameSceneName = "Kolman";
        private readonly string _mainMenuScene = "MainMenu";

        private readonly int _minPlayers = 2;
        private readonly int _maxPlayers = 2;
        
        private Dictionary<ulong, GameObject> _playersReady;

        public override void OnNetworkSpawn()
        {
            _playersReady =new Dictionary<ulong, GameObject>();
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnClientLoadScene;
                startGameButton.GetComponent<CanvasGroup>().alpha = 1;
                startGameButton.GetComponent<CanvasGroup>().interactable = true;
                startGameButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            ConnectionManager.Instance.PlayersInGame.OnListChanged += OnPlayersInGameChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            RefreshScreen();
        }

        private void RefreshScreen()
        {
            GenerateScreen();
            UpdateScreen();
            CheckAllPlayersReadyServerRpc();
        }

        private void OnPlayersInGameChanged(NetworkListEvent<Player> e)
        {
            RefreshScreen();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AddPlayerToLobbyServerRpc(ulong clientId, FixedString128Bytes playerName)
        {
            AddPlayer(clientId, playerName.Value);
        }
        
        [ClientRpc]
        private void AddPlayerToLobbyClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
        {
            string playerName = ConnectionManager.Instance.PlayerName;
            AddPlayerToLobbyServerRpc(clientId, playerName);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnClientLoadScene;
            }
            
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            ConnectionManager.Instance.PlayersInGame.OnListChanged -= OnPlayersInGameChanged;
        }

        /*public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (IsServer)
            {
                if (NetworkManager.Singleton.SceneManager != null)
                    NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnClientLoadScene;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
            
            ConnectionManager.Instance.PlayersInGame.OnListChanged -= OnPlayersInGameChanged;
        }*/

        private void OnClientDisconnect(ulong clientId)
        {
            // Server disconnects client
            if (!IsServer && (clientId == NetworkManager.Singleton.LocalClientId || NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId == clientId))
            {
                Debug.Log("Client disconnected from server");
                NetworkManager.Singleton.Shutdown();
                ClientCloseSession();
                return;
            }
            if (_playersReady.ContainsKey(clientId))
            {
                Destroy(_playersReady[clientId]);
                _playersReady.Remove(clientId);
                RemovePlayer(clientId);
            }
        }
        
        public void OnClientLoadScene(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] {clientId}
                }
            };
            AddPlayerToLobbyClientRpc(clientId, clientRpcParams);
        }

        private void GenerateUserForLobby(Player player)
        {
            GameObject playerLobby = Instantiate(playerInLobbyPrefab, playersInLobbyWrapper.transform);
            // Get the Name child of the playerLobby
            TextMeshProUGUI playerName = playerLobby.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerName.text = player.Name.Value;
            // Get the State child of the playerLobby
            TextMeshProUGUI playerReady = playerLobby.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            playerReady.text = "Not Ready";
            _playersReady.Add(player.Id, playerLobby);
            if (IsHost)
                CheckAllPlayersReadyServerRpc();
        }

        private void GenerateAllUsersForLobby()
        {
            foreach (Player player in ConnectionManager.Instance.PlayersInGame)
            {
                if (_playersReady.ContainsKey(player.Id))
                    continue;
                GenerateUserForLobby(player);
            }
        }
        
        private void ClickReadyHandle(ulong clientId)
        {
            GameObject playerLobby = _playersReady[clientId];
            // Get the State child of the playerLobby
            TextMeshProUGUI playerReady = playerLobby.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Player player = GetPlayer(clientId);
            player.IsReady = !player.IsReady;
            SetPlayer(player);
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

            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnClientLoadScene;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            SceneTransitionHandler.sceneTransitionHandler.RegisterGameCallbacks();
            //Transition to the ingame scene
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_gameSceneName);
        }

        private bool CheckAllPlayersReady()
        {
            if (ConnectionManager.Instance.PlayersInGame.Count >= _minPlayers)
            {
                foreach (Player player in ConnectionManager.Instance.PlayersInGame)
                {
                    if (!player.IsReady)
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
            startGameButton.GetComponent<CanvasGroup>().alpha = 0.5f;
            startGameButton.GetComponent<CanvasGroup>().interactable = false;
            startGameButton.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnClientIsReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            if (_playersReady.ContainsKey(clientId))
            {
                ClickReadyHandle(clientId);
                //UpdateScreenClientRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void CheckAllPlayersReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!CheckAllPlayersReady()) return;
            this.startGameButton.interactable = true;
            this.startGameButton.GetComponent<CanvasGroup>().alpha = 1;
            this.startGameButton.GetComponent<CanvasGroup>().interactable = true;
            this.startGameButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        
        private void GenerateScreen()
        {
            GenerateAllUsersForLobby();
        }
        
        private void UpdateScreen()
        {
            foreach (KeyValuePair<ulong, GameObject> pUI in _playersReady)
            {
                Player player = GetPlayer(pUI.Key);
                // Get the State child of the playerLobby
                TextMeshProUGUI playerReady = pUI.Value.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                playerReady.text = player.IsReady ? "Ready" : "Not Ready";
            }
        }
        
        public void OnClose()
        {
            SoundManager.Instance.PlayButtonClickSound(Camera.main.transform.position);
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnClientLoadScene;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                /*foreach (Player player in ConnectionManager.Instance.PlayersInGame)
                {
                    if (player.Id == NetworkManager.Singleton.LocalClientId) continue;
                    NetworkManager.Singleton.DisconnectClient(player.Id);
                }*/
                ConnectionManager.Instance.DisconnectGameServerRpc();
            }
            NetworkManager.Singleton.Shutdown();
            ClientCloseSession();
        }

        /**
         * Client process to remove any event listeners and clean up the scene and go back to the main menu
         */
        private void ClientCloseSession()
        {
            ConnectionManager.Instance.PlayersInGame.OnListChanged-= OnPlayersInGameChanged;
            SceneManager.LoadScene(_mainMenuScene);
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
        
        public void RemovePlayer(ulong clientId)
        {
            // Remove player from list
            ConnectionManager.Instance.PlayersInGame.Remove(new Player
            {
                Id = clientId
            });
        }

        public Player GetPlayer(ulong clientId)
        {
            int index = ConnectionManager.Instance.PlayersInGame.IndexOf(new Player
            {
                Id = clientId
            });
            if (index != -1)
            {
                Player player = ConnectionManager.Instance.PlayersInGame[index];
                return player;
            }

            return new Player()
            {
                Id = 615
            };
        }

        public void SetPlayer(Player player)
        {
            int index = ConnectionManager.Instance.PlayersInGame.IndexOf(new Player
            {
                Id = player.Id
            });
            if (index != -1)
            {
                ConnectionManager.Instance.PlayersInGame[index] = player;
            }
        }

        public void AddPlayer(ulong clientId, string playerName)
        {
            Player player = new Player
            {
                Id = clientId,
                Name = string.IsNullOrEmpty(playerName) ? "Player_" + clientId  : playerName,
                IsReady = false
            };
            ConnectionManager.Instance.PlayersInGame.Add(player);
        }
    }
}