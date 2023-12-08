using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ScaryCrowChaseState : State
    {
        private ScaryCrow scaryCrow;
        private float timer;
        public ScaryCrowChaseState(ScaryCrow controller, string stateName) : base(controller, stateName)
        {
            scaryCrow = controller;
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
                scaryCrow.SwitchState(scaryCrow.attackState);
                return;
            }
            Collider2D[] colls = scaryCrow.GetTargetsInView();
            if (colls.Length == 0) return;
            if (Vector3.SqrMagnitude(controller.transform.position - colls[0].transform.position) < 16)
            {
                scaryCrow.SwitchState(scaryCrow.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            Collider2D[] colls = scaryCrow.GetTargetsInView();
            if (colls.Length == 0) return;
            scaryCrow.HandleLockTarget(colls[0].transform,0.5f);
            scaryCrow.moveAmount = 2;
            scaryCrow.core.movement.SetVelocityX(7 * scaryCrow.core.movement.facingDirection);
        }
    }
}

