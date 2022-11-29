﻿using System;
using Kolman_Freecss.HitboxHurtboxSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Kolman_Freecss.Krodun
{
    public class EnemyAI : AbstractEnemyAI
    {
        
        protected override void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("moving");
            _animIDIdle = Animator.StringToHash("battle");
        }
        
        protected override void Update()
        {
            if (health.IsDead())
            {
                enabled = false;
                navMeshAgent.enabled = false;
            }
            
            // Idle State by default
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
                _animator.SetInteger(_animIDIdle, 1); // Idle animation
            }

            distanceToTarget = Vector3.Distance(_player.position, transform.position);

            if (isProvoked)
            {
                EngageTarget();
            }
            else if (distanceToTarget <= chaseRange)
            {
                isProvoked = true;
            }
        }
        
        protected override void StopAttack()
        {
            StopAnimationHit();
            StopAnimationHit2();
        }
        
        protected override void AttackTarget()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 5); // Attack animation
            }
        }
        
        protected override void TriggerAnimationMove()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 2); // Run animation
            }
        }

        protected override void TriggerAnimationIdle()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 1); // Run animation
            }
        }

        protected override void TriggerAnimationHit2()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 5); // Attack animation
            }
        }

        protected override void TriggerAnimationDeath()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 6); // Death animation
            }
        }

        protected override void TriggerAnimationHit()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 4); // Hit animation
            }
        }

        protected override void StopAnimationMove()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
            }
        }

        protected override void StopAnimationIdle()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDIdle, 0); // Stop animation
            }
        }

        protected override void StopAnimationHit2()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
            }
        }

        protected override void StopAnimationDeath()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
            }
        }

        protected override void StopAnimationHit()
        {
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDMoving, 0); // Stop animation
            }
        }
    }
}