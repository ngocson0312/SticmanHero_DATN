using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMechaIdleState : State
    {
        private GreenMecha greenMecha;
        private float timer;
        public GreenMechaIdleState(GreenMecha controller, string stateName) : base(controller, stateName)
        {
            greenMecha = controller;
        }

        public override void EnterState()
        {
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
                    greenMecha.SwitchState(greenMecha.attackState);
                }
                else
                {
                    greenMecha.SwitchState(greenMecha.chaseState);
                }
            }
        }

        public override void UpdatePhysic()
        {
            WalkAround();
        }
        void WalkAround()
        {
            Controller target = greenMecha.GetTargetsInView();
            greenMecha.moveAmount = 0;
            if (!target) return;
            greenMecha.HandleLockTarget(target.transform, 0.5f);
            if (Vector3.Distance(target.transform.position, controller.transform.position) <= 3)
            {
                greenMecha.core.movement.SetVelocityX(0);
                return;
            }
            greenMecha.moveAmount = 1;
            greenMecha.core.movement.SetVelocityX(greenMecha.core.movement.facingDirection * 3);
        }
    }

}
