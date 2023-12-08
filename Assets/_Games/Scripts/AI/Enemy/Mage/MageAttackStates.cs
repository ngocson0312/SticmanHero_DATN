using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class MageAttackStates : State
    {
        BossRedWitch bossRedWitch;
        float attackTimer = 0;
        private float timer;
        private float dodgeBackTimer;
        int currentDirection;
        bool checkEnemy;
        float TimerVfx = 0;

        public MageAttackStates(BossRedWitch controller, string stateName) : base(controller, stateName)
        {
            this.bossRedWitch = controller;

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
            // Controller target = bossRedWitch.GetTargetInView();
            // if (target == null)
            // {
            //     return;
            // }
            // Vector3 direction = (target.position - controller.position);
            // if (direction.x < -0.1f)
            // {
            //     currentDirection = -1;
            // }
            // else if (direction.x > 0.1f)
            // {
            //     currentDirection = 1;
            // }
            // if (currentDirection != controller.core.movement.facingDirection && controller.isInteracting)
            // {
            //     controller.core.movement.Flip();
            // }
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 3f;
                bossRedWitch.CiticalVfx.Play();
                bossRedWitch.animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);
            }

        }

        public override void UpdatePhysic()
        {

        }
    }
}
