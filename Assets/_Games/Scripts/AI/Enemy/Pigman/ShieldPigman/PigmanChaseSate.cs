using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigmanChaseSate : State
    {
        private PigmanShield enemy;
        private int currentDirection;
        private float timer;
        public PigmanChaseSate(PigmanShield controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
        }



        public override void EnterState()
        {
            timer = 1f;
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {

        }


        public override void UpdatePhysic()
        {
            Controller target = enemy.GetTargetInView();

            if (target == null)
            {
                enemy.SwitchState(enemy.pigmanShieldPatrol);
            }
            else
            {
                Vector3 direction = (target.transform.position - controller.transform.position);
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
                timer -= Time.fixedDeltaTime;
                if (timer <= 0)
                {
                    Vector2 force = new Vector2(8, 10);
                    force.x *= core.movement.facingDirection;
                    core.movement.SetVelocity(force);
                    timer = 1.5f;
                }
                if (direction.magnitude <= enemy.attackRange)
                {

                    enemy.SwitchState(enemy.pigmanShieldAttackState);
                }
            }
        }
    }
}
