using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HornetRequiemAttackState : State
    {
        private HornetRequiem hornetRequiem;
        private float timer;
        public HornetRequiemAttackState(HornetRequiem controller, string stateName) : base(controller, stateName)
        {
            hornetRequiem = controller;
        }

        public override void EnterState()
        {
            Controller colls = hornetRequiem.GetTargetsInView();
            hornetRequiem.moveAmount = 0;
            controller.core.movement.SetVelocityX(0);
            if (colls == null)
            {
                hornetRequiem.SwitchState(hornetRequiem.idleState);
                return;
            }
            Transform target = colls.transform;
            hornetRequiem.HandleLockTarget(target, 1f);
            float distance = Vector3.Distance(controller.transform.position, target.position);
            if (distance > 5)
            {
                int r = Random.Range(0, 100);
                if (r <= 30)
                {
                    hornetRequiem.AddAction(new HRFlyAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRComboAction(hornetRequiem, target, 4));
                    timer = 12;
                }
                else if (r > 30 && r <= 60)
                {
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRComboAction(hornetRequiem, target, 4));
                    timer = 8;
                }
                else
                {
                    hornetRequiem.AddAction(new HRFlyAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    timer = 8;
                }
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r < 30)
                {
                    hornetRequiem.AddAction(new HRComboAction(hornetRequiem, target, 4));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRFlyAttackAction(hornetRequiem, target));
                    timer = 11;
                }
                else if (r >= 30 && r < 60)
                {
                    hornetRequiem.AddAction(new HRFlyAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    timer = 8;
                }
                else
                {
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRComboAction(hornetRequiem, target, 4));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    hornetRequiem.AddAction(new HRTeleportAttackAction(hornetRequiem, target));
                    timer = 13;
                }
            }
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            Controller colls = hornetRequiem.GetTargetsInView();
            if (colls == null && hornetRequiem.rotateWhenAction)
            {
                hornetRequiem.HandleLockTarget(colls.transform, 1f);
            }
            if (timer <= 0)
            {
                hornetRequiem.SwitchState(hornetRequiem.idleState);
            }
        }

        public override void UpdatePhysic()
        {
        }
    }
}

