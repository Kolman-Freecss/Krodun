using Kolman_Freecss.HitboxHurtboxSystem;
using Kolman_Freecss.QuestSystem;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] float damage = 40f;
        [SerializeField] float health = 100f;
        [SerializeField] AmountType enemyType = AmountType.TROLL;
        
        bool _isDead = false;
        PlayerBehaviour _player;
        
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
            _player = FindObjectOfType<PlayerBehaviour>();
            AssignAnimationIDs();
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
            _player.EventQuest(EventQuestType.Kill, enemyType);
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