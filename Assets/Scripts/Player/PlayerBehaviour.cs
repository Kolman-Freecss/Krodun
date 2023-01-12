using System;
using System.Collections;
using Kolman_Freecss.QuestSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Kolman_Freecss.Krodun
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] float health = 1000f;
        private float maxHealth;
        private Image _healthBar;
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
        [SerializeField]
        private ParticleSystem _hurtParticles;

        private void Awake()
        {
            maxHealth = health;
            SubscribeToDelegatesAndUpdateValues();
        }
        
        private void SubscribeToDelegatesAndUpdateValues()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        public void OnGameStarted(bool isLoaded, ulong clientId)
        {
            if (isLoaded && clientId == NetworkManager.Singleton.LocalClientId)
            {
                _healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
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
            _krodunController.TakeDamage(health, maxHealth);
            _hurtParticles.Play();
            if (health <= 0 && !_isDead)
            {
                HandleDeath();
            }
        }
        
        public void Heal(InventoryItem item)
        {
            health += item.healthAmount;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            _krodunController.Heal(health, maxHealth);
        }
        
        public void HandleDeath()
        {
            _isDead = true;
            StartCoroutine(HandleDeathCoroutine());
        }

        IEnumerator HandleDeathCoroutine()
        {
            yield return new WaitForSeconds(DelayToDeath);
            GameManager.Instance.PlayerDeath();
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