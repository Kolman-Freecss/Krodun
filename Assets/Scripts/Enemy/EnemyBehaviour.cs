﻿using System;
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
        [SerializeField] 
        public NetworkVariable<float> health = new NetworkVariable<float>(100f, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);
        [SerializeField] 
        AmountType enemyType = AmountType.TROLL;
        
        [HideInInspector]
        public NetworkVariable<bool> _isDead = new NetworkVariable<bool>(false, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);
        
        protected List<PlayerBehaviour> _players;
        protected PlayerBehaviour _playerTarget;
        
        // animation IDs
        private int _animIDMoving;
        private int _animIDIdle;
        private int _animIDDeath;
        
        private bool _hasAnimator;
        private Animator _animator;
        
        private EnemyHitbox _hitbox;

        private void Awake()
        {
            SubscribeToDelegatesAndUpdateValues();
        }

        void Start()
        {
            _hitbox = GetComponentInChildren<EnemyHitbox>();
            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();
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
            TakeDamageServerRpc(damage);
            
            if (health.Value <= 0) return;
            BroadcastMessage("OnDamageTaken");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(int damage)
        {
            health.Value -= damage;
            if (health.Value <= 0)
            {
                Die();
            }
        }
        
        // Only called on the server
        void Die()
        {
            if (!IsServer || _isDead.Value) return;
            _isDead.Value = true;
            // TODO Event to all players
            DieClientRpc();
        }
        
        [ClientRpc]
        void DieClientRpc()
        {
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
            return _isDead.Value;
        }
        
    }
}