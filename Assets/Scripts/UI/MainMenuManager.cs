using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class MainMenuManager : MonoBehaviour
    {
        private GameObject _mainMenuManager;
        private static MainMenuManager Instance { get; set; }

        private void Awake()
        {
            if (_mainMenuManager == null)
            {
                _mainMenuManager = GameObject.FindGameObjectWithTag("MainMenuManager");
            }

            ManageSingleton();
        }

        void ManageSingleton()
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
    }
}