using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HornetRequiemChaseState : State
    {
        private HornetRequiem hornetRequiem;
        private float timer;
        public HornetRequiemChaseState(HornetRequiem controller, string stateName) : base(controller, stateName)
        {
            hornetRequiem = controller;
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
                hornetRequiem.SwitchState(hornetRequiem.attackState);
                return;
            }
            Controller colls = hornetRequiem.GetTargetsInView();
            if (colls == null) return;
            if (Vector3.SqrMagnitude(controller.transform.position - colls.transform.position) <= 9)
            {
                hornetRequiem.SwitchState(hornetRequiem.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            Controller colls = hornetRequiem.GetTargetsInView();
            if (colls == null) return;
            hornetRequiem.HandleLockTarget(colls.transform, 0.5f);
            if (Vector3.SqrMagnitude(colls.transform.position - controller.transform.position) <= 9)
            {
                hornetRequiem.moveAmount = 0;
                hornetRequiem.core.movement.SetVelocityX(0);
                return;
            }
            hornetRequiem.moveAmount = 1;
            hornetRequiem.core.movement.SetVelocityX(7 * hornetRequiem.core.movement.facingDirection);
        }
    }

}
