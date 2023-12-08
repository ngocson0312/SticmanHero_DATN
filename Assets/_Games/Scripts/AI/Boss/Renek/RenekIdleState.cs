using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekIdleState : State
    {
        private Renek renek;
        private float timer;
        public RenekIdleState(Renek controller, string stateName) : base(controller, stateName)
        {
            renek = controller;
        }

        public override void EnterState()
        {
            timer = Random.Range(1f, 2f);
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (Random.Range(0, 100) < 50)
                {
                    renek.SwitchState(renek.attackState);
                }
                else
                {
                    renek.SwitchState(renek.chaseState);
                }
            }

        }

        public override void UpdatePhysic()
        {
            WalkAround();
        }
        void WalkAround()
        {
            Controller controller = renek.GetTargetsInView();
            renek.moveAmount = 0;
            if (controller == null) return;
            if (Vector3.SqrMagnitude(controller.transform.position - controller.transform.position) <= 9)
            {
                renek.core.movement.SetVelocityX(0);
                return;
            }
            renek.moveAmount = 1;
            renek.HandleLockTarget(controller.transform, 0.3f);
            renek.core.movement.SetVelocityX(renek.core.movement.facingDirection * 3);
        }
    }
}

