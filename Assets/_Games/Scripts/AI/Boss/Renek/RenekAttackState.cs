using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekAttackState : State
    {
        private Renek renek;
        private float timer;
        public RenekAttackState(Renek controller, string stateName) : base(controller, stateName)
        {
            renek = controller;
        }

        public override void EnterState()
        {
            Controller target = renek.GetTargetsInView();
            renek.moveAmount = 0;
            controller.core.movement.SetVelocityX(0);
            if (target == null)
            {
                renek.SwitchState(renek.idleState);
                return;
            }
            renek.HandleLockTarget(target.transform, 1f);
            float distance = Vector3.SqrMagnitude(controller.transform.position - target.position);
            if (distance <= 4)
            {
                renek.SwitchState(renek.pursuitState);
                return;
            }
            if (distance > 25)
            {
                renek.AddAction(new RenekChargeAction(renek, target.transform));
                renek.AddAction(new RenekChargeAction(renek, target.transform));
                timer = 5;
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r < 30)
                {
                    renek.AddAction(new RenekAttackAction(renek, "Attack1", 1.5f));
                    timer = 1.5f;
                }
                else if (r >= 30 && r < 65)
                {
                    renek.AddAction(new RenekAttackAction(renek, "Attack1", 1.5f));
                    renek.AddAction(new RenekAttackAction(renek, "Attack3", 1.5f));
                    timer = 3;
                }
                else
                {
                    renek.AddAction(new RenekChargeAction(renek, target.transform));
                    renek.AddAction(new RenekAttackAction(renek, "Attack3", 1.5f));
                    timer = 4;
                }
            }


        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                renek.SwitchState(renek.idleState);
            }
        }

        public override void UpdatePhysic()
        {

        }
    }
}

