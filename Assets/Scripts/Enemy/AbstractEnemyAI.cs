using Kolman_Freecss.HitboxHurtboxSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Kolman_Freecss.Krodun
{
    public abstract class AbstractEnemyAI : MonoBehaviour
    {
        [SerializeField] protected float chaseRange = 10f;
        [SerializeField] protected float turnSpeed = 5f;
        protected NavMeshAgent navMeshAgent;
        protected float distanceToTarget = Mathf.Infinity;
        protected bool isProvoked = false;
        protected EnemyBehaviour health;
        protected Transform _player;
        
        // Animator
        protected Animator _animator;
        protected bool _hasAnimator;
        
        // animation IDs
        protected int _animIDMoving;
        protected int _animIDIdle;
        
        protected EnemyHitbox _hitbox;
        protected EnemyHurtbox _hurtbox;
        
        private bool _isAttacking;
        
        public event IHitboxResponder.FacingDirectionChanged OnFacingDirectionChangedHitbox;
        public event IHurtboxResponder.FacingDirectionChanged OnFacingDirectionChangedHurtbox;

        private void Awake()
        {
            _hitbox = GetComponentInChildren<EnemyHitbox>();
            _hurtbox = GetComponentInChildren<EnemyHurtbox>();
            OnFacingDirectionChangedHitbox += _hitbox.OnFacingDirectionChangedHandler;
            OnFacingDirectionChangedHurtbox += _hurtbox.OnFacingDirectionChangedHandler;
        }
        
        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyBehaviour>();
            _player = FindObjectOfType<KrodunController>().transform;
            AssignAnimationIDs();
            
            // set our initial facing direction hitbox
            OnFacingDirectionChangedHitbox?.Invoke(transform);
            OnFacingDirectionChangedHurtbox?.Invoke(transform);
        }

        protected abstract void AssignAnimationIDs();

        protected abstract void Update();
        
        protected abstract void AttackTarget();

        // Called by Unity when the enemy is hit by a weapon (TakeDamage)
        public void OnDamageTaken()
        {
            isProvoked = true;
        }

        protected void EngageTarget()
        {
            FaceTarget();
            if (_isAttacking)
            {
                StopAttack();
            }

            if (distanceToTarget >= navMeshAgent.stoppingDistance)
            {
                ChaseTarget();
            }

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                _isAttacking = true;
                AttackTarget();
            }
        }

        protected void FaceTarget()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        void ChaseTarget()
        {
            TriggerAnimationMove();
            navMeshAgent.SetDestination(_player.position);
            // set our facing direction hitbox
            OnFacingDirectionChangedHitbox?.Invoke(transform);
            OnFacingDirectionChangedHurtbox?.Invoke(transform);
        }

        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
        
        protected abstract void StopAttack();
        
        protected abstract void TriggerAnimationMove();
        
        protected abstract void TriggerAnimationIdle();
        
        protected abstract void TriggerAnimationHit2();
        
        protected abstract void TriggerAnimationDeath();
        
        protected abstract void TriggerAnimationHit();
        
        //protected abstract void TriggerAnimationStunned();

        protected abstract void StopAnimationMove();
        
        protected abstract void StopAnimationIdle();
        
        protected abstract void StopAnimationHit2();
        
        protected abstract void StopAnimationDeath();
        
        protected abstract void StopAnimationHit();
        
    }
}