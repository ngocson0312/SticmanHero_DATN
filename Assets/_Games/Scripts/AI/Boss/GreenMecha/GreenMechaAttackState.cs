using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMechaAttackState : State
    {
        private GreenMecha greenMecha;
        private float timer;
        public GreenMechaAttackState(GreenMecha controller, string stateName) : base(controller, stateName)
        {
            greenMecha = controller;
        }

        public override void EnterState()
        {
            Controller targetx = greenMecha.GetTargetsInView();
            greenMecha.moveAmount = 0;
            controller.core.movement.SetVelocityX(0);
            if (!targetx)
            {
                greenMecha.SwitchState(greenMecha.idleState);
                return;
            }
            Transform target = targetx.transform;
            greenMecha.HandleLockTarget(target, 1);
            float distance = Vector3.Distance(controller.transform.position, target.position);
            if (distance > 8)
            {
                if (Random.Range(0, 100) < 50)
                {

                    greenMecha.AddAction(new GreenMechaAttackAction(greenMecha, "ATK2", 2.5f));
                    timer = 3;
                }
                else
                {
                    greenMecha.SwitchState(greenMecha.chaseState);
                    return;
                }
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r < 30)
                {
                    greenMecha.AddAction(new GreenMechaAttackAction(greenMecha, "ATK1", 3f));
                    greenMecha.AddAction(new GroundSpikeAction(greenMecha, target));
                    timer = 6;
                }
                else if (r >= 30 && r < 65)
                {
                    greenMecha.AddAction(new GreenMechaAttackAction(greenMecha, "ATK1", 3f));
                    greenMecha.AddAction(new GroundSpikeAction(greenMecha, target));
                    timer = 6;
                }
                else
                {
                    greenMecha.AddAction(new GreenMechaAttackAction(greenMecha, "ATK2", 2.5f));
                    greenMecha.AddAction(new GroundSpikeAction(greenMecha, target));
                    timer = 6;
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
                greenMecha.SwitchState(greenMecha.idleState);
            }
        }

        public override void UpdatePhysic()
        {

        }
    }
}