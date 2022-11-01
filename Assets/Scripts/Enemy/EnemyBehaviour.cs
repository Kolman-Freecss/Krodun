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
        
        void Start()
        {
            _player = FindObjectOfType<PlayerBehaviour>();
            AssignAnimationIDs();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("moving");
            _animIDIdle = Animator.StringToHash("battle");
        }
        
        public void TakeDamage(float damage)
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
            //GetComponent<Animator>().SetTrigger("die");
            //TODO death vfx and Invoke to Die animation
            Destroy(gameObject);
        }
        
        // Method assigned to the animation event
        public void AttackHitEvent()
        {
            Debug.Log("Hit");
            if (_player == null) return;
            _player.TakeDamage(damage);
            //_player.GetComponent<DisplayDamage>().ShowDamageImpact();
        }
        
        public bool IsDead()
        {
            return _isDead;
        }
        
    }
}