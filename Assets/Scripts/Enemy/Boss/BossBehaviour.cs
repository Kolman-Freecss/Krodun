using Kolman_Freecss.Krodun;
using UnityEngine;

namespace DefaultNamespace
{
    public class BossBehaviour : MonoBehaviour
    {
        private EnemyBehaviour _enemyBehaviour;
        
        private void Start()
        {
            _enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        }
        
        // Method assigned to the animation event
        public void AttackHitEvent()
        {
            _enemyBehaviour.AttackHitEvent();
        }
    }
}