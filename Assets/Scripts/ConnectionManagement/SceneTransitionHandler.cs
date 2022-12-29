using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class SceneTransitionHandler : NetworkBehaviour
    {
        static public SceneTransitionHandler sceneTransitionHandler { get; internal set; }
        
        [SerializeField]
        public string DefaultMainMenu = "MainMenu";
        
        [HideInInspector]
        public delegate void ClientLoadedSceneDelegateHandler(ulong clientId);
        [HideInInspector]
        public event ClientLoadedSceneDelegateHandler OnClientLoadedGameScene;
        
        [HideInInspector]
        public delegate void ClientLoadedSceneCountDelegateHandler(ulong clientId);
        [HideInInspector]
        public event ClientLoadedSceneCountDelegateHandler OnClientLoadedGameSceneCount;
        
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
            Kolman,
            GameOver
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

        private void ManageSingleton()
        {
            if (sceneTransitionHandler != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                sceneTransitionHandler = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        private void Start()
        {
            if(m_SceneState == SceneStates.Init)
                
            {
                SceneManager.LoadScene(DefaultMainMenu);
            }
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
        
        public void ExitAndLoadStartMenu()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
            OnClientLoadedGameScene = null;
            SetSceneState(SceneStates.Start);
            SceneManager.LoadScene(1);
        }
        
        public void RegisterGameCallbacks()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }
        
        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if ("Kolman".Equals(sceneName))
            {
                m_numberOfClientLoaded += 1;
                OnClientLoadedGameScene?.Invoke(clientId);
            }
            else
            {
                OnClientLoadedGameSceneCount?.Invoke(clientId);
            }
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