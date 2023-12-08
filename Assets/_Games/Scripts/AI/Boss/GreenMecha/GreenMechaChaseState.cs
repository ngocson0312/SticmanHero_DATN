using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMechaChaseState : State
    {
        private float timer;
        private GreenMecha greenMecha;
        public GreenMechaChaseState(GreenMecha controller, string stateName) : base(controller, stateName)
        {
            greenMecha = controller;
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
                greenMecha.SwitchState(greenMecha.attackState);
                return;
            }
            Controller target = greenMecha.GetTargetsInView();
            if (!target) return;
            if (Vector3.Distance(target.transform.position, controller.transform.position) < 3)
            {
                greenMecha.SwitchState(greenMecha.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            Controller target = greenMecha.GetTargetsInView();
            if (!target) return;
            greenMecha.HandleLockTarget(target.transform, 0.5f);
            if (Vector3.Distance(target.transform.position , controller.transform.position) <= 3)
            {
                greenMecha.moveAmount = 0;
                greenMecha.core.movement.SetVelocityX(0);
                return;
            }
            greenMecha.moveAmount = 1;
            greenMecha.core.movement.SetVelocityX(3 * greenMecha.core.movement.facingDirection);
        }
    }

}
