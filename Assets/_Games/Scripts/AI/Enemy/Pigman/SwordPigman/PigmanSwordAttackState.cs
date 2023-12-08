using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigmanSwordAttackState : State
    {
        private PigManSword pigManSword;
        private float timer;
        private float dodgeBackTimer;
        private int currentDirection;
        public PigmanSwordAttackState(PigManSword controller, string stateName) : base(controller, stateName)
        {
            pigManSword = controller;
            pigManSword.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        public override void EnterState()
        {
            timer = 0.5f;
            core.movement.SetVelocityX(0);
        }

        ~PigmanSwordAttackState()
        {

            pigManSword.animatorHandle.OnEventAnimation -= CriticalEvent;
        }

        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                pigManSword.CiticalVfx.Play();
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
            Controller target = pigManSword.GetTargetInView();
            if (target == null)
            {
                pigManSword.SwitchState(pigManSword.pigmanSwordPatrol);
                return;
            }

            if (!controller.core.collisionSenses.GroundAhead() || controller.core.collisionSenses.IsTouchWall())
            {
                pigManSword.core.movement.Flip();
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
            timer -= Time.deltaTime;
            dodgeBackTimer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 1f;
                pigManSword.weapon.TriggerWeapon();
            }
        }

        public override void UpdatePhysic()
        {

        }

    }
}
