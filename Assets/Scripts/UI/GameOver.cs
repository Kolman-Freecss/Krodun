using System;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun
{
    public class GameOver : MonoBehaviour
    {
        private void Awake()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedGameSceneCount += OnClientLoadedGameSceneCount;
            }
        }

        private void OnClientLoadedGameSceneCount(ulong clientId)
        {
            ConnectionManager.Instance.playersInSceneCount++;
            if (ConnectionManager.Instance.playersInSceneCount == ConnectionManager.Instance.playersInGameCount)
            {
                SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedGameSceneCount -= OnClientLoadedGameSceneCount;
                ConnectionManager.Instance.GameOver();
            }
        }

        public void OnExitButtonPressed()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}