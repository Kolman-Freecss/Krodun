using System;
using System.Collections;
using Kolman_Freecss.QuestSystem;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] float health = 1000f;
        public int defaultDamage = 40;
        private bool _isDead = false;
        public bool IsDead
        {
            get { return _isDead; }
        }
        int _experience = 0;
        private const float DelayToDeath = 6f;
        private static PlayerBehaviour Instance { get; set; }
        private KrodunController _krodunController;
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
            Init();
        }

        public void Init()
        {
            _experience = 0;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            //_player.GetComponent<DisplayDamage>().ShowDamageImpact();
            if (health <= 0 && !_isDead)
            {
                HandleDeath();
            }
        }
        
        public void HandleDeath()
        {
            _isDead = true;
            StartCoroutine(HandleDeathCoroutine());
            /*Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;*/
        }

        IEnumerator HandleDeathCoroutine()
        {
            yield return new WaitForSeconds(DelayToDeath);
            GameManager.Instance.isGameOver.Value = true;
        }

        public void EventQuest(EventQuestType eventQuestType, AmountType amountType)
        {
            switch (eventQuestType)
            {
                case EventQuestType.Kill:
                    AddExperience(10);
                    QuestManager.Instance.EventTriggered(EventQuestType.Kill, amountType);                    
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