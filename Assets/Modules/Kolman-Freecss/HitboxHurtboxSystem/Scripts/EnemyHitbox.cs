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
    [RequireComponent(typeof(BoxCollider))]
    public class EnemyHitbox : BasicBehaviourHitbox
    {
        
        
        public void OnTriggerEnter(Collider other)
        {
            if (LayerMask.GetMask("PlayerHitbox") == other.gameObject.layer)
            {
                Debug.Log("Hitbox: " + gameObject.name + " has hit " + other.gameObject.name);
                Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                hurtbox?.OnHit(this);
            }
        }

        public void OnTriggerStay(Collider other)
        {
            Debug.Log("Hitbox: " + gameObject.name + " is still hitting " + other.gameObject.name);
            /*if (LayerMask.GetMask("Enemy") == other.gameObject.layer)
            {
                Debug.Log("Hitbox: " + gameObject.name + " has hit " + other.gameObject.name);
                Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                hurtbox?.OnHit(this);
            }*/
        }

        public void OnTriggerExit(Collider other)
        {
            Debug.Log("Hitbox: " + gameObject.name + " has exited " + other.gameObject.name);
            /*if (other.CompareTag("Hurtbox"))
            {
                Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                hurtbox.OnExitHit(this);
            }*/
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
        
    }
}