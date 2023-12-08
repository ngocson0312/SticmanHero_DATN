using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekChaseState : State
    {
        private Renek renek;
        private float timer;
        public RenekChaseState(Renek controller, string stateName) : base(controller, stateName)
        {
            renek = controller;
        }

        public override void EnterState()
        {
            timer = Random.Range(1f, 3f);
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                renek.SwitchState(renek.attackState);
                return;
            }
            Controller target = renek.GetTargetsInView();
            if (target == null) return;
            if (Vector3.SqrMagnitude(controller.transform.position - target.transform.position) < 9)
            {
                renek.SwitchState(renek.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            Controller target = renek.GetTargetsInView();
            if (target == null) return;
            renek.HandleLockTarget(target.transform, 0.5f);
            if (Vector3.SqrMagnitude(target.transform.position - controller.transform.position) <= 9)
            {
                renek.moveAmount = 0;
                renek.core.movement.SetVelocityX(0);
                return;
            }
            renek.moveAmount = 2;
            renek.core.movement.SetVelocityX(7 * renek.core.movement.facingDirection);
        }
    }
}

