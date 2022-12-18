using System.Collections.Generic;
using System.Linq;
using Kolman_Freecss.HitboxHurtboxSystem;
using Kolman_Freecss.QuestSystem;
using Unity.Netcode;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class EnemyBehaviour : NetworkBehaviour
    {
        [SerializeField] float damage = 40f;
        [SerializeField] float health = 100f;
        [SerializeField] AmountType enemyType = AmountType.TROLL;
        
        bool _isDead = false;
        
        protected List<PlayerBehaviour> _players;
        protected PlayerBehaviour _playerTarget;
        
        // animation IDs
        private int _animIDMoving;
        private int _animIDIdle;
        private int _animIDDeath;
        
        private bool _hasAnimator;
        private Animator _animator;
        
        private EnemyHitbox _hitbox;
        
        void Start()
        {
            _hitbox = GetComponentInChildren<EnemyHitbox>();
            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();
            SubscribeToDelegatesAndUpdateValues();
        }
        
        private void SubscribeToDelegatesAndUpdateValues()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        public void OnGameStarted(bool isLoaded)
        {
            if (isLoaded)
            {
                _players = FindObjectsOfType<PlayerBehaviour>().ToList();
                _playerTarget = _players[0].GetComponent<PlayerBehaviour>();
            }
        }
        
        private void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("moving");
            _animIDIdle = Animator.StringToHash("battle");
            _animIDDeath = Animator.StringToHash("death");
        }
        
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                BroadcastMessage("OnDamageTaken");
            }
        }
        
        void Die()
        {
            if (_isDead) return;
            _isDead = true;
            // TODO Event to all players
            _playerTarget.EventQuest(EventQuestType.Kill, enemyType);
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDDeath);
            }
            Invoke("Destroy", 3f);
        }
        
        void Destroy()
        {
            Destroy(gameObject);
        }
        
        // Method assigned to the animation event
        public void AttackHitEvent()
        {
            if (_hitbox.InHitbox)
            {
                // TODO Event to all players
                if (_playerTarget == null) return;
                _playerTarget.TakeDamage(damage);
            }
        }
        
        public bool IsDead()
        {
            
            return _isDead;
        }
        
    }
}