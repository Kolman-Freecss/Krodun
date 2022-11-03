using UnityEngine;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    public class BasicBehaviourHitbox : MonoBehaviour
    {
        private float offset = 1f;
        
        public virtual void OnFacingDirectionChangedHandler(Transform target)
        {
            //transform.localPosition = new Vector3(target.localPosition.x, transform.localPosition.y, transform.localPosition.z) * offset;
        }
    }
}