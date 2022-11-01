using UnityEngine;
using UnityEngine.AI;

namespace Kolman_Freecss.Krodun
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRange = 10f;
        [SerializeField] float turnSpeed = 5f;

        NavMeshAgent navMeshAgent;
        float distanceToTarget = Mathf.Infinity;
        bool isProvoked = false;
        EnemyBehaviour health;
        Transform _player;
        
        // Animator
        private Animator _animator;
        private bool _hasAnimator;
        
        // animation IDs
        private int _animIDMoving;
        private int _animIDIdle;

        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyBehaviour>();
            _player = FindObjectOfType<KrodunController>().transform;
            AssignAnimationIDs();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("moving");
            _animIDIdle = Animator.StringToHash("battle");
        }

        void Update()
        {
            if (health.IsDead())
            {
                enabled = false;
                navMeshAgent.enabled = false;
            }
            
            // Idle State by default
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
                _animator.SetInteger(_animIDIdle, 1); // Idle animation
            }

            distanceToTarget = Vector3.Distance(_player.position, transform.position);

            if (isProvoked)
            {
                EngageTarget();
            }
            else if (distanceToTarget <= chaseRange)
            {
                isProvoked = true;
            }
        }

        public void OnDamageTaken()
        {
            isProvoked = true;
        }

        void EngageTarget()
        {
            FaceTarget();
            if (distanceToTarget >= navMeshAgent.stoppingDistance)
            {
                ChaseTarget();
            }

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                AttackTarget();
            }
        }

        void AttackTarget()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 5); // Attack animation
            }
        }

        void FaceTarget()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        void ChaseTarget()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 2); // Run animation
            }
            navMeshAgent.SetDestination(_player.position);
        }

        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}