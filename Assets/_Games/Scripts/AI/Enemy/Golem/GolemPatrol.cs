using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class GolemPatrol : State
    {
        Golem enemy;
        public GolemPatrol(Golem controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
        }

        public override void EnterState()
        {
           enemy.moveAmount = 1;
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            if (target != null)
            {
                enemy.SwitchState(enemy.golemAttack);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            if (controller.isStunning || controller.isInteracting) return;
            PatrolAround();
        }
        void PatrolAround()
        {
            if (controller.core.collisionSenses.IsOnGround())
            {
                if (!controller.core.collisionSenses.GroundAhead() || controller.core.collisionSenses.IsTouchWall())
                {
                    enemy.core.movement.Flip();
                }
                controller.core.movement.SetVelocityX(controller.core.movement.facingDirection * enemy.speed);
            }
        }
    }
}
