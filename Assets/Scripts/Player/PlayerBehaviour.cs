using System;
using Kolman_Freecss.QuestSystem;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] float health = 1000f;
        public int damage = 100;
        int _experience = 0;
        private static PlayerBehaviour Instance { get; set; }
        private KrodunController _krodunController;
        private QuestManager _questManager;
        public KrodunController KrodunController
        {
            get
            {
                return _krodunController;
            }
        }

        private void Start()
        {
            _krodunController = GetComponent<KrodunController>();
            _questManager = GetComponent<QuestManager>();
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
            //_player.GetComponent<DisplayDamage>().ShowDamageImpact();
            if (health <= 0)
            {
                HandleDeath();
            }
        }
        
        public void HandleDeath()
        {
            gameOverCanvas.enabled = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void EventQuest(EventQuestType eventQuestType, AmountType amountType)
        {
            switch (eventQuestType)
            {
                case EventQuestType.Kill:
                    AddExperience(10);
                    _questManager.EventTriggered(EventQuestType.Kill, amountType);                    
                    break;
                default:
                    break;
            }
            
        }
        
        private void AddExperience(int experience)
        {
            _experience += experience;
        }
        
    }
}