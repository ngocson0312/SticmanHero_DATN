using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigmanSwordChase : State
    {
        private PigManSword enemy;
        private int currentDirection;
        public PigmanSwordChase(PigManSword controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
        }

        public override void EnterState()
        {
            enemy.moveAmount = 2;
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
            enemy.moveAmount = 2;
            if (!controller.core.collisionSenses.IsOnGround())
            {
                return;
            }
            if (target == null)
            {
                enemy.SwitchState(enemy.pigmanSwordPatrol);
            }
            else
            {
                Vector2 direction = (target.transform.position - controller.transform.position);
                if (direction.x < -0.1f)
                {
                    currentDirection = -1;
                }
                else if (direction.x > 0.1f)
                {
                    currentDirection = 1;
                }
                int faceDir = controller.core.movement.facingDirection;
                if (currentDirection != faceDir && !controller.isInteracting)
                {
                    controller.core.movement.Flip();
                }

                controller.core.movement.SetVelocityX(controller.core.movement.facingDirection * enemy.speed * 2);
                if (currentDirection == faceDir)
                {
                    if (!controller.core.collisionSenses.GroundAhead() || controller.core.collisionSenses.IsTouchWall())
                    {
                        enemy.moveAmount = 0;
                        controller.core.movement.SetVelocityX(0);
                    }
                }

                if (direction.magnitude <= enemy.attackRange)
                {
                    enemy.SwitchState(enemy.pigmanSwordAttack);
                }
            }
        }
    }
}
