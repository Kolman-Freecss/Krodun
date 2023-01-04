using System.Collections.Generic;
using System.Linq;
using Kolman_Freecss.HitboxHurtboxSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Kolman_Freecss.Krodun
{
    public abstract class AbstractEnemyAI : NetworkBehaviour
    {
        #region ###### Variables ######

        [SerializeField] protected float chaseRange = 10f;
        [SerializeField] protected float turnSpeed = 5f;
        protected NavMeshAgent navMeshAgent;
        protected float distanceToTarget = Mathf.Infinity;
        protected bool isProvoked = false;
        protected EnemyBehaviour health;
        
        protected List<Transform> _players;
        protected Transform _playerTarget;
        
        // Animator
        protected Animator _animator;
        protected bool _hasAnimator;
        
        // animation IDs
        protected int _animIDMoving;
        protected int _animIDIdle;
        
        protected EnemyHitbox _hitbox;
        protected EnemyHurtbox _hurtbox;
        
        private bool _isAttacking;
        protected bool _gameStarted = false;

        #endregion
        
        public event IHitboxResponder.FacingDirectionChanged OnFacingDirectionChangedHitbox;
        public event IHurtboxResponder.FacingDirectionChanged OnFacingDirectionChangedHurtbox;

        private void Awake()
        {
            _hitbox = GetComponentInChildren<EnemyHitbox>();
            _hurtbox = GetComponentInChildren<EnemyHurtbox>();
            OnFacingDirectionChangedHitbox += _hitbox.OnFacingDirectionChangedHandler;
            OnFacingDirectionChangedHurtbox += _hurtbox.OnFacingDirectionChangedHandler;
            SubscribeToDelegatesAndUpdateValues();
        }
        
        void Start()
        {
            // TryGetComponent in child components
            _hasAnimator = TryGetComponent(out _animator);
            if (!_hasAnimator)
            {
                _animator = GetComponentInChildren<Animator>();
                if (_animator != null)
                {
                    _hasAnimator = true;
                }
            }
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyBehaviour>();
            AssignAnimationIDs();
            
            // set our initial facing direction hitbox
            OnFacingDirectionChangedHitbox?.Invoke(transform);
            OnFacingDirectionChangedHurtbox?.Invoke(transform);
            
        }
        
        private void SubscribeToDelegatesAndUpdateValues()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        
        public void OnGameStarted(bool isLoaded, ulong clientId)
        {
            if (isLoaded)
            {
                _players = FindObjectsOfType<KrodunController>().ToList().ConvertAll(x => x.transform);
                _playerTarget = _players[0];
                _gameStarted = isLoaded;
            }
        }
        
        private void OnClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"Client {clientId} connected");
            _players = FindObjectsOfType<KrodunController>().ToList().ConvertAll(x => x.transform);
        }


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
            
            distanceToTarget = Vector3.Distance(_playerTarget.position, transform.position);

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
            if (transform == null) return;
            Vector3 direction = (_playerTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        void ChaseTarget()
        {
            TriggerAnimationMove();
            navMeshAgent.SetDestination(_playerTarget.position);
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
        
        public override void OnDestroy()
        {
            if (!IsServer) return;
            base.OnDestroy();
            
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                if (networkObject.IsSpawned)
                    networkObject.Despawn();
            }
        }

        #region ###### Abstract Methods ######
        
        protected abstract void AssignAnimationIDs();

        protected abstract void Update();
        
        protected abstract void AttackTarget();

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

        #endregion
        
    }
}