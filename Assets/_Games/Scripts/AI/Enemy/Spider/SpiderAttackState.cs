using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class SpiderAttackState : State
    {
        Spider enemy;
        float attackTimer = 0;
        int currentDirection;
        public SpiderAttackState(Spider controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
            enemy.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        public override void EnterState()
        {

            currentDirection = enemy.core.movement.facingDirection;
            attackTimer = 0.3f;
        }
        public override void ExitState()
        {

        }
        ~SpiderAttackState()
        {

            enemy.animatorHandle.OnEventAnimation -= CriticalEvent;
        }
        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                enemy.CiticalVfx.Play();
            }
            // if (eventName.Equals("CriticalFalse"))
            // {
            //     enemy.Citical.SetActive(false);
            // }
        }



        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            enemy.core.movement.SetVelocityX(0);
            if (target == null)
            {
                enemy.SwitchState(enemy.spiderPatrolState);
                return;
            }
            if (Vector2.SqrMagnitude(target.transform.position - controller.transform.position) > enemy.attackRange && !controller.isInteracting)
            {
                enemy.SwitchState(enemy.spiderChaseState);
                return;
            }

            if (!controller.isInteracting)
            {
                Vector2 dir = target.transform.position - controller.transform.position;
                if (dir.x < 0)
                {
                    currentDirection = -1;
                }
                else if (dir.x > 0)
                {
                    currentDirection = 1;
                }
                if (currentDirection != controller.core.movement.facingDirection && !controller.isInteracting)
                {

                    controller.core.movement.Flip();
                }
            }

            if (attackTimer >= 0)
            {

                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    enemy.weapon.TriggerWeapon();
                    attackTimer = 1 / enemy.attackSpeed;
                }
            }

        }

        public override void UpdatePhysic()
        {

        }
    }
}
