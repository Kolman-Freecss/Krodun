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
        Transform target;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyBehaviour>();
            target = FindObjectOfType<PlayerBehaviour>().transform;
        }

        void Update()
        {
            if (health.IsDead())
            {
                enabled = false;
                navMeshAgent.enabled = false;
            }

            distanceToTarget = Vector3.Distance(target.position, transform.position);

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
            GetComponent<Animator>().SetBool("attack", true);
        }

        void FaceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        void ChaseTarget()
        {
            GetComponent<Animator>().SetBool("attack", false);
            GetComponent<Animator>().SetTrigger("move");
            navMeshAgent.SetDestination(target.position);
        }

        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}