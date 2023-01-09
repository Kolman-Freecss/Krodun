using System;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class FollowCharacter : MonoBehaviour
    {
        [SerializeField] private MinimapBehaviour _minimapBehaviour;
        [SerializeField] private float cameraHeight;

        private void Awake()
        {
            _minimapBehaviour = GetComponentInParent<MinimapBehaviour>();
            cameraHeight = transform.position.y;
        }

        void LateUpdate()
        {
            if (_minimapBehaviour == null || _minimapBehaviour.targetToFollow == null) return;
            Vector3 targetPosition = _minimapBehaviour.targetToFollow.transform.position;
            transform.position = new Vector3(targetPosition.x,  targetPosition.y + cameraHeight, targetPosition.z);
            if (_minimapBehaviour.rotateWithTarget)
            {
                Quaternion targetRotation = _minimapBehaviour.targetToFollow.transform.rotation;
                transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
            }
        }
    }
}