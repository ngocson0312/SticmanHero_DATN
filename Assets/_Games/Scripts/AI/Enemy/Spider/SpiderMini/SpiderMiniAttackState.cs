using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class SpiderMiniAttackState : State
    {
        MiniSpider enemy;
        float attackTimer = 0;
        int currentDirection;
        public SpiderMiniAttackState(MiniSpider controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
            enemy.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        ~SpiderMiniAttackState()
        {
            enemy.animatorHandle.OnEventAnimation -= CriticalEvent;
        }
        public override void EnterState()
        {
            currentDirection = enemy.core.movement.facingDirection;
            attackTimer = 0.5f;
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
        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            enemy.core.movement.SetVelocityX(0);
            if (target == null)
            {
                enemy.SwitchState(enemy.spiderMiniPatrolState);
                return;
            }
            if (Vector2.Distance(target.transform.position, controller.transform.position) > enemy.attackRange && !controller.isInteracting)
            {
                enemy.SwitchState(enemy.spiderMiniChaseState);
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
                if (!controller.isInteracting)
                {
                    if (currentDirection != controller.core.movement.facingDirection)
                    {
                        controller.core.movement.Flip();

                    }
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
