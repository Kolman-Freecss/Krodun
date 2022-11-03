using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    /*
     * <summary>
     *   This class is used to associate it to a box collider that will be the hurtbox.
     * </summary>
     */
    public class Hurtbox : BasicBehaviourHitbox
    {
        public void OnHit(EnemyHitbox hitbox)
        {  
            Debug.Log("Hurtbox.OnHit");
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}