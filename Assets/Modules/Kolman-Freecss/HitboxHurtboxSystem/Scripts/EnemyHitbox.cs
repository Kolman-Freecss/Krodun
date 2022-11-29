using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    /*
     * <summary>
     *   This class is used to associate it to a box collider that will be the hitbox.
     * </summary>
     */
    public class EnemyHitbox : BasicBehaviourHitbox
    {
        public bool InHitbox { get; private set; }

        private void Start()
        {
            InHitbox = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (LayerMask.NameToLayer("PlayerHurtbox") == other.gameObject.layer)
            {
                InHitbox = true;
                //_enemyBehaviour.OnHitboxEnter(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (LayerMask.NameToLayer("PlayerHurtbox") == other.gameObject.layer)
            {
                InHitbox = false;
                //_enemyBehaviour.OnHitboxEnter(other);
            }
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
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