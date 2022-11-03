using System.Diagnostics;
using Kolman_Freecss.Krodun;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    /*
     * <summary>
     *   This class is used to associate it to a box collider that will be the hitbox.
     * </summary>
     */
    public class Hitbox : BasicBehaviourHitbox
    {

        public bool InHitbox { get; private set; }
        public Collider CurrentCollider { get; private set; }

        private PlayerBehaviour _playerBehaviour;

        private void Start()
        {
            _playerBehaviour = FindObjectOfType<PlayerBehaviour>();
        }

        public void Attack()
        {
            if (InHitbox)
            {
                EnemyHurtbox hurtbox = CurrentCollider.GetComponent<EnemyHurtbox>();
                hurtbox?.OnHit(_playerBehaviour.damage);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (LayerMask.NameToLayer("EnemyHurtbox") == other.gameObject.layer)
            {
                InHitbox = true;
                CurrentCollider = other;
                //_enemyBehaviour.OnHitboxEnter(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (LayerMask.NameToLayer("EnemyHurtbox") == other.gameObject.layer)
            {
                InHitbox = false;
                CurrentCollider = null;
                //_enemyBehaviour.OnHitboxEnter(other);
            }
        }
        
        [Conditional("DEBUG")]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
        
    }
}