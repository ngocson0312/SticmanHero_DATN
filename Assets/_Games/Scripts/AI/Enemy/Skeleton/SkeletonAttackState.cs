using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class SkeletonAttackState : State
    {
        private Skeleton skeleton;
        private float timer;
        private float dodgeBackTimer;
        private int currentDirection;
        public SkeletonAttackState(Skeleton controller, string stateName) : base(controller, stateName)
        {
            skeleton = controller;
            skeleton.animatorHandle.OnEventAnimation += OnEvent;
            skeleton.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        ~SkeletonAttackState()
        {
            skeleton.animatorHandle.OnEventAnimation -= OnEvent;
            skeleton.animatorHandle.OnEventAnimation -= CriticalEvent;
        }

        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                skeleton.CiticalVfx.Play();
            }
            // if (eventName.Equals("CriticalFalse"))
            // {
            //     enemy.Citical.SetActive(false);
            // }
        }

        private void OnEvent(string eventName)
        {
            if (eventName.Equals("Jump"))
            {
                Vector2 force = new Vector2(8, 8);
                force.x *= -core.movement.facingDirection;
                core.movement.SetVelocity(force);
            }
        }

        public override void EnterState()
        {
            core.movement.SetVelocityX(0);
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            Controller target = skeleton.GetTargetInView();
            if (target == null)
            {
                skeleton.SwitchState(skeleton.skeletonPatrolState);
                return;
            }
            Vector3 direction = (target.position - controller.position);
            if (direction.x < -0.1f)
            {
                currentDirection = -1;
            }
            else if (direction.x > 0.1f)
            {
                currentDirection = 1;
            }
            if (currentDirection != controller.core.movement.facingDirection && !controller.isInteracting)
            {
                controller.core.movement.Flip();
            }
            timer -= Time.deltaTime;
            dodgeBackTimer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 2f;
                skeleton.weapon.TriggerWeapon();
            }
            else
            {
                float distance = Vector2.Distance(transform.position, target.position);
                if (dodgeBackTimer <= 0 && distance <= 3 && !controller.isInteracting)
                {
                    dodgeBackTimer = 2;
                    skeleton.animatorHandle.PlayAnimation("JumpBack", 0.1f, 1);
                }
            }
        }
        public override void UpdatePhysic()
        {

        }
    }
}
