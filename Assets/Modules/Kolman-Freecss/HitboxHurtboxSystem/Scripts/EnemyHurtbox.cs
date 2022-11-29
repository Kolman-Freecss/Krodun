using System.Diagnostics;
using Kolman_Freecss.Krodun;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    /*
     * <summary>
     *   This class is used to associate it to a box collider that will be the hurtbox.
     * </summary>
     */
    public class EnemyHurtbox : BasicBehaviourHitbox
    {
        private EnemyBehaviour _enemyBehaviour;

        private void Start()
        {
            _enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        }
        
        public void OnHit(int damage)
        {  
            _enemyBehaviour.TakeDamage(damage);
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            if (this.GetComponent<Collider>() is BoxCollider boxCollider)
            {
                Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
            }
            else if (this.GetComponent<Collider>() is SphereCollider sphereCollider)
            {
                Gizmos.DrawWireSphere(sphereCollider.bounds.center, sphereCollider.radius);
            }
        }
    }
}