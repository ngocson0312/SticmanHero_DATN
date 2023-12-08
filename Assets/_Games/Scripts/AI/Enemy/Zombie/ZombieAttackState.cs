using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ZombieAttackState : State
    {
        Zombie enemy;
        float attackTimer = 0;
        int currentDirection;
        public ZombieAttackState(Zombie controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
            enemy.animatorHandle.OnEventAnimation += CriticalEvent;

        }
        public override void EnterState()
        {
            currentDirection = enemy.core.movement.facingDirection;
            attackTimer = 0.3f;
        }
        ~ZombieAttackState()
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

        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            enemy.core.movement.SetVelocityX(0);
            if (target == null)
            {
                enemy.SwitchState(enemy.zombiePatrolState);
                return;
            }
            Vector2 dir = target.transform.position - controller.transform.position;
            if (dir.magnitude > enemy.attackRange && !controller.isInteracting)
            {
                enemy.SwitchState(enemy.zombiePatrolState);
                return;
            }

            if (!controller.isInteracting)
            {
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
                if (attackTimer <= 0 && !controller.isInteracting && !controller.isStunning)
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
