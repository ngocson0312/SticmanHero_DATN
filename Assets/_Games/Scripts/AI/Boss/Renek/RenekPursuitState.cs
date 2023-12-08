using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class RenekPursuitState : State
    {
        private Renek renek;
        private int state;
        private float timer;
        public RenekPursuitState(Renek controller, string stateName) : base(controller, stateName)
        {
            renek = controller;
        }

        public override void EnterState()
        {
            state = 0;
            timer = 0;
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            Controller target = renek.GetTargetsInView();
            if (!target) return;
            if (Vector3.SqrMagnitude(controller.transform.position - target.transform.position) > 25 && state == 0)
            {
                state = 1;
            }
            if (state == 1)
            {
                renek.HandleLockTarget(target.transform, 1f);
                renek.SwitchState(renek.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            renek.moveAmount = 2;
            renek.core.movement.SetVelocityX(7 * renek.core.movement.facingDirection);
            if (renek.core.collisionSenses.IsTouchWall())
            {
                renek.core.movement.Flip();
            }
            if (renek.core.movement.facingDirection < 0)
            {
                renek.animatorHandle.transform.localRotation = Quaternion.Lerp(renek.animatorHandle.transform.localRotation, Quaternion.Euler(0, 270, 0), 0.5f);
            }
            if (renek.core.movement.facingDirection > 0)
            {
                renek.animatorHandle.transform.localRotation = Quaternion.Lerp(renek.animatorHandle.transform.localRotation, Quaternion.Euler(0, 100, 0), 0.5f);
            }
        }
    }
}