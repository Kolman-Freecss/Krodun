﻿using Kolman_Freecss.HitboxHurtboxSystem;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] float damage = 40f;
        [SerializeField] float health = 100f;
        
        bool _isDead = false;
        PlayerBehaviour _player;
        
        // animation IDs
        private int _animIDMoving;
        private int _animIDIdle;
        
        private bool _hasAnimator;
        private Animator _animator;
        
        private EnemyHitbox _hitbox;
        
        void Start()
        {
            _hitbox = GetComponentInChildren<EnemyHitbox>();
            _hasAnimator = TryGetComponent(out _animator);
            _player = FindObjectOfType<PlayerBehaviour>();
            AssignAnimationIDs();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("moving");
            _animIDIdle = Animator.StringToHash("battle");
        }
        
        public void TakeDamage(int damage)
        {
            BroadcastMessage("OnDamageTaken");
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            if (_isDead) return;
            _isDead = true;
            _player.AddExperience(10);
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 9);
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
                if (_player == null) return;
                _player.TakeDamage(damage);
            }
        }
        
        public bool IsDead()
        {
            
            return _isDead;
        }
        
    }
}