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
    public class EnemyHurtbox : BasicBehaviourHitbox
    {
        public void OnHit(Hitbox hitbox)
        {  
            Debug.Log("EnemyHurtbox.OnHit");
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}