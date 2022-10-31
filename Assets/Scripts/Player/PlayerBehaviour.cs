using System;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] float health = 100f;
        int _experience = 0;
        private static PlayerBehaviour Instance { get; set; }

        private void Awake()
        {
            ManageSingleton();
        }

        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            _experience = 0;
            if (gameOverCanvas != null)
                gameOverCanvas.enabled = false;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                HandleDeath();
            }
        }
        
        public void AddExperience(int experience)
        {
            _experience += experience;
        }
        
        public void HandleDeath()
        {
            gameOverCanvas.enabled = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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