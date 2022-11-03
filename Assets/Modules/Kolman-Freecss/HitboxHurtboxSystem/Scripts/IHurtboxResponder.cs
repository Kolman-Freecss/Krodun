using UnityEngine;

namespace Kolman_Freecss.HitboxHurtboxSystem
{
    public interface IHurtboxResponder
    {
        
        // Delegates and events for Hitbox
        public delegate void FacingDirectionChanged(Transform target);
        public event FacingDirectionChanged OnFacingDirectionChangedHurtbox;
        
    }
}