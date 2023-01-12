using System;
using Kolman_Freecss.Krodun;
using UnityEngine;

namespace NPC
{
    public class NPCBehaviour : MonoBehaviour
    {
        private Animator _animator;
        private bool _hasAnimator;
        
        private int _animIDIdle;
        
        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (!_hasAnimator)
            {
                _animator = GetComponentInChildren<Animator>();
                if (_animator != null)
                {
                    _hasAnimator = true;
                }
            }
            AssignAnimationIDs();
        }

        private void Update()
        {
            
            if (_hasAnimator && !GameManager.Instance.isGameOver.Value)
            {
                _animator.SetBool(_animIDIdle, true);
            } else if (_hasAnimator && GameManager.Instance.isGameOver.Value)
            {
                _animator.SetBool(_animIDIdle, false);
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDIdle = Animator.StringToHash("Idle");
        }
    }
}