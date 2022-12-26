using System;
using System.Collections.Generic;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Model;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
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
        private string _gameSceneName = "Kolman";
        private string _mainMenuScene = "MainMenu";

        private int minPlayers = 2;
        private int maxPlayers = 2;
        
        public NetworkList<Player> PlayersInGame;
        private Dictionary<ulong, GameObject> _playersReady = new Dictionary<ulong, GameObject>();

        private void Awake()
        {
            PlayersInGame  = new NetworkList<Player>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnClientLoadScene;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                startGameButton.GetComponent<CanvasGroup>().alpha = 1;
                startGameButton.GetComponent<CanvasGroup>().interactable = true;
                startGameButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AddPlayerToLobbyServerRpc(ulong clientId)
        {
            PlayersInGame.Add(new Player
            {
                Id = clientId,
                /*playerName*/
                IsReady = false
            });
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnClientLoadScene;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        private void OnClientDisconnect(ulong obj)
        {
            if (_playersReady.ContainsKey(obj))
            {
                Destroy(_playersReady[obj]);
                _playersReady.Remove(obj);
                RemovePlayer(obj);
            }
        }
        
        public void OnClientLoadScene(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            AddPlayerToLobbyServerRpc(clientId);
            GenerateScreenClientRpc();
            UpdateScreenClientRpc();
        }

        private void GenerateUserForLobby(Player player)
        {
            GameObject playerLobby = Instantiate(playerInLobbyPrefab, playersInLobbyWrapper.transform);
            // Get the Name child of the playerLobby
            TextMeshProUGUI playerName = playerLobby.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerName.text = "Player_" + player.Id;
            // Get the State child of the playerLobby
            TextMeshProUGUI playerReady = playerLobby.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            playerReady.text = "Not Ready";
            _playersReady.Add(player.Id, playerLobby);
            if (IsHost)
                CheckAllPlayersReadyServerRpc();
        }

        private void GenerateAllUsersForLobby()
        {
            foreach (Player player in PlayersInGame)
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
            if (PlayersInGame.Count >= minPlayers)
            {
                foreach (Player player in PlayersInGame)
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
                UpdateScreenClientRpc();
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
        
        [ClientRpc]
        private void GenerateScreenClientRpc()
        {
            GenerateAllUsersForLobby();
        }
        
        [ClientRpc]
        private void UpdateScreenClientRpc()
        {
            UpdateScreen();
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
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                // Close the server
                NetworkManager.Singleton.Shutdown();
                //SceneTransitionHandler.sceneTransitionHandler.SwitchScene(_mainMenuScene);
            }
            else
            {
                // Disconnect from the server
                ClientDisconnectServerRpc();
            }
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
            PlayersInGame.Remove(new Player
            {
                Id = clientId
            });
        }

        public Player GetPlayer(ulong clientId)
        {
            int index = PlayersInGame.IndexOf(new Player
            {
                Id = clientId
            });
            if (index != -1)
            {
                Player player = PlayersInGame[index];
                return player;
            }

            return new Player()
            {
                Id = 615
            };
        }

        public void SetPlayer(Player player)
        {
            int index = PlayersInGame.IndexOf(new Player
            {
                Id = player.Id
            });
            if (index != -1)
            {
                PlayersInGame[index] = player;
            }
        }
    }
}