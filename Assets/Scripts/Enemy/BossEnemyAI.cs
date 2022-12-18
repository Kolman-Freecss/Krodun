using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class BossEnemyAI : AbstractEnemyAI
    {
        protected int _animIDHit;
        protected int _animIDDeath;
        protected int _animIDHit2;

        protected override void AssignAnimationIDs()
        {
            _animIDMoving = Animator.StringToHash("Walk");
            _animIDIdle = Animator.StringToHash("IdleAction");
            _animIDHit = Animator.StringToHash("Hit");
            _animIDDeath = Animator.StringToHash("Death");
            _animIDHit2 = Animator.StringToHash("Hit2");
        }

        protected override void Update()
        {
            if (!_gameStarted) return;
            if (health.IsDead())
            {
                enabled = false;
                navMeshAgent.enabled = false;
            }

            StopAnimationMove();

            // TODO Change that calculation to a OnTrigger
            // Check if any player is in range
            //distanceToTarget = Vector3.Distance(_player.position, transform.position);

            if (isProvoked)
            {
                EngageTarget();
            }
            if (_players.Any(player => Vector3.Distance(player.transform.position, transform.position) <= chaseRange))
            {
                isProvoked = true;
                _playerTarget = _players.First(player => Vector3.Distance(player.transform.position, transform.position) <= chaseRange);
            }
        }
        
        protected override void AttackTarget()
        {
            if (_hasAnimator)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
                    !_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit2"))
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        TriggerAnimationHit();
                    }
                    else
                    {
                        TriggerAnimationHit2();
                    }
                }
            }
        }

        protected override void StopAttack()
        {
            StopAnimationHit();
            StopAnimationHit2();
        }

        protected override void TriggerAnimationMove()
        {
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDMoving, 1);
            }
        }

        protected override void TriggerAnimationIdle()
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDIdle);
            }
        }

        protected override void TriggerAnimationHit2()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHit2, true);
            }
        }

        protected override void TriggerAnimationDeath()
        {
            if (_hasAnimator)
            {
                _animator.SetTrigger(_animIDDeath);
            }
        }

        protected override void TriggerAnimationHit()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHit, true);
            }
        }

        protected override void StopAnimationMove()
        {
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDMoving, 0);
            }
        }

        protected override void StopAnimationIdle()
        {
            if (_hasAnimator)
            {
                _animator.ResetTrigger(_animIDIdle);
            }
        }

        protected override void StopAnimationHit2()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHit2, false);
            }
        }

        protected override void StopAnimationDeath()
        {
            if (_hasAnimator)
            {
                _animator.ResetTrigger(_animIDDeath);
            }
        }

        protected override void StopAnimationHit()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHit, false);
            }
        }
    }
}