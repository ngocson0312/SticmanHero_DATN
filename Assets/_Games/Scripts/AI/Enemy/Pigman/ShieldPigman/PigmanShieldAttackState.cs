using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigmanShieldAttackState : State
    {

        private PigmanShield pigmanShield;
        private float timer;
        private float dodgeBackTimer;
        private int currentDirection;
        public PigmanShieldAttackState(PigmanShield controller, string stateName) : base(controller, stateName)
        {
            pigmanShield = controller;
            pigmanShield.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        public override void EnterState()
        {
            core.movement.SetVelocityX(0);

        }
        ~PigmanShieldAttackState()
        {

            pigmanShield.animatorHandle.OnEventAnimation -= CriticalEvent;
        }
        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                pigmanShield.CiticalVfx.Play();
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
            Controller target = pigmanShield.GetTargetInView();
            if (target == null)
            {
                pigmanShield.SwitchState(pigmanShield.pigmanShieldPatrol);
                return;
            }

            if (!controller.isInteracting)
            {
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

            }

            timer -= Time.deltaTime;
            dodgeBackTimer -= Time.deltaTime;
            if (timer <= 0)
            {

                Vector2 force = new Vector2(3, 6);
                force.x *= core.movement.facingDirection;
                core.movement.SetVelocity(force);
                pigmanShield.weapon.TriggerWeapon();
                timer = 1f;
            }
        }


        public override void UpdatePhysic()
        {

        }

    }
}
