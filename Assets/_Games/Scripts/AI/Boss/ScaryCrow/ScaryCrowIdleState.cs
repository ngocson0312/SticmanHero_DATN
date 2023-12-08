using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ScaryCrowIdleState : State
    {
        private ScaryCrow scaryCrow;
        private float timer;
        int currentDirection;
        public ScaryCrowIdleState(ScaryCrow controller, string stateName) : base(controller, stateName)
        {
            scaryCrow = controller;
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
            if (timer <= 0)
            {
                if (Random.Range(0, 100) > 50)
                {
                    scaryCrow.SwitchState(scaryCrow.attackState);
                }
                else
                {
                    scaryCrow.SwitchState(scaryCrow.chaseState);
                }
                return;
            }
        }

        public override void UpdatePhysic()
        {
            WalkAround();
        }
        void WalkAround()
        {
            Collider2D[] colls = scaryCrow.GetTargetsInView();
            scaryCrow.moveAmount = 0;
            if (colls.Length == 0 || controller.isInteracting) return;
            if (Vector3.SqrMagnitude(colls[0].transform.position - controller.transform.position) <= 9)
            {
                scaryCrow.core.movement.SetVelocityX(0);
                return;
            }
            scaryCrow.moveAmount = 1;
            scaryCrow.HandleLockTarget(colls[0].transform, 0.5f);
            scaryCrow.core.movement.SetVelocityX(scaryCrow.core.movement.facingDirection * 3);
        }
    }
}