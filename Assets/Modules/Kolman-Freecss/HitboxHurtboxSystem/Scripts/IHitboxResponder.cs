using UnityEngine;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    public interface IHitboxResponder
    {
        
        // Delegates and events for Hitbox
        public delegate void FacingDirectionChanged(Transform target);
        public event FacingDirectionChanged OnFacingDirectionChangedHitbox;
        
        //void OnHit(Hitbox hitbox, Collider collider, Vector3 hitPoint);
    }
}