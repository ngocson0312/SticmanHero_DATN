using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class WitherSkeltonAttackState : State
    {
        WitherSkelton enemy;
        float attackTimer = 0;
        float TimerVfx = 0;
        int currentDirection;
        public WitherSkeltonAttackState(WitherSkelton controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
            // enemy.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        public override void EnterState()
        {
            currentDirection = enemy.core.movement.facingDirection;
            attackTimer = 0.5f;
        }
        // ~WitherSkeltonAttackState()
        // {

        //     enemy.animatorHandle.OnEventAnimation -= CriticalEvent;
        // }
        // private void CriticalEvent(string eventName)
        // {
        //     if (eventName.Equals("CriticalTrue"))
        //     {
        //         enemy.Citical.SetActive(true);
        //     }
        //     if (eventName.Equals("CriticalFalse"))
        //     {
        //         enemy.Citical.SetActive(false);
        //     }
        // }
        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            enemy.core.movement.SetVelocityX(0);
            if (target == null)
            {
                return;
            }
            if (Vector2.Distance(target.transform.position , controller.transform.position) > enemy.attackRange)
            {
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
            if (currentDirection != controller.core.movement.facingDirection && controller.isInteracting)
            {
                controller.core.movement.Flip();
            }

            if (attackTimer >= 0)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    enemy.weapon.TriggerWeapon();
                    attackTimer = 1 / enemy.attackSpeed;
                    TimerVfx -= Time.deltaTime;
                    if (TimerVfx <= 0)
                    {
                        enemy.CiticalVfx.Play();
                        TimerVfx = 0.025f;
                    }

                }
            }
        }

        public override void UpdatePhysic()
        {

        }
    }
}
