using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class SceneTransitionHandler : NetworkBehaviour
    {
        static public SceneTransitionHandler sceneTransitionHandler { get; internal set; }
        
        [HideInInspector]
        public delegate void ClientLoadedSceneDelegateHandler(ulong clientId);
        [HideInInspector]
        public event ClientLoadedSceneDelegateHandler OnClientLoadedScene;
        
        [HideInInspector]
        public delegate void SceneStateChangedDelegateHandler(SceneStates newState);
        [HideInInspector]
        public event SceneStateChangedDelegateHandler OnSceneStateChanged;
        
        private int m_numberOfClientLoaded;
        
        public enum SceneStates
        {
            Init,
            Start,
            Lobby,
            Ingame
        }
        
        private SceneStates m_SceneState;
        
        private void Awake()
        {
            if(sceneTransitionHandler != this && sceneTransitionHandler != null)
            {
                GameObject.Destroy(sceneTransitionHandler.gameObject);
            }
            sceneTransitionHandler = this;
            SetSceneState(SceneStates.Init);
            DontDestroyOnLoad(this);
        }
        
        public void SetSceneState(SceneStates sceneState)
        {
            m_SceneState = sceneState;
            if(OnSceneStateChanged != null)
            {
                OnSceneStateChanged.Invoke(m_SceneState);
            }
        }
        
        public SceneStates GetCurrentSceneState()
        {
            return m_SceneState;
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                if (sceneTransitionHandler != null)
                {
                    NetworkManager.Singleton.SceneManager.UnloadScene(sceneTransitionHandler.gameObject.scene);
                }
                sceneTransitionHandler = this;
            }
        }
        
        public void ExitAndLoadStartMenu()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
            OnClientLoadedScene = null;
            SetSceneState(SceneStates.Start);
            SceneManager.LoadScene(1);
        }
        
        public void RegisterCallbacks()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }
        
        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            m_numberOfClientLoaded += 1;
            OnClientLoadedScene?.Invoke(clientId);
        }
        
        public void SwitchScene(string scenename)
        {
            if(NetworkManager.Singleton.IsListening)
            {
                m_numberOfClientLoaded = 0;
                NetworkManager.Singleton.SceneManager.LoadScene(scenename, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadSceneAsync(scenename);
            }
        }
        
        public bool AllClientsAreLoaded()
        {
            return m_numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count;
        }
    }
}