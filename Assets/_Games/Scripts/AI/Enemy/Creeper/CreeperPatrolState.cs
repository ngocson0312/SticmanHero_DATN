using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class CreeperPatrolState : State
    {
         Creeper creeper;
        public CreeperPatrolState(Creeper controller, string stateName) : base(controller, stateName)
        {
            this.creeper = controller;
        }

        public override void EnterState()
        {
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            Controller target = creeper.GetTargetInView();
            if (target != null)
            {
                creeper.SwitchState(creeper.creeperChaseState);
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
                    creeper.core.movement.Flip();
                }
                controller.core.movement.SetVelocityX(controller.core.movement.facingDirection * creeper.speed);
            }
        }
    }
}
