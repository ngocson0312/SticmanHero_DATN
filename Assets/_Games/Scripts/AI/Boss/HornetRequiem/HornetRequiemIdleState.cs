using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HornetRequiemIdleState : State
    {
        private HornetRequiem hornetRequiem;
        private float timer;
        public HornetRequiemIdleState(HornetRequiem controller, string stateName) : base(controller, stateName)
        {
            hornetRequiem = controller;
        }

        public override void EnterState()
        {
            timer = Random.Range(0.2f, 1f);
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            Controller coll = hornetRequiem.GetTargetsInView();
            if (!coll) return;
            hornetRequiem.HandleLockTarget(coll.transform, 0.5f);
            if (timer <= 0)
            {
                if (Random.Range(0, 100) < 50)
                {
                    hornetRequiem.SwitchState(hornetRequiem.attackState);
                }
                else
                {
                    hornetRequiem.SwitchState(hornetRequiem.chaseState);
                }
            }

        }

        public override void UpdatePhysic()
        {

        }
    }
}

