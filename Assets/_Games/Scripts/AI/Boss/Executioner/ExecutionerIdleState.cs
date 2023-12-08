using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ExecutionerIdleState : State
    {
        Executioner executioner;
        float idleTimer;
        int currentDirection;
        public ExecutionerIdleState(Executioner controller, string stateName) : base(controller, stateName)
        {
            executioner = controller;
        }

        public override void EnterState()
        {
            currentDirection = executioner.core.movement.facingDirection;
            idleTimer = Random.Range(1f, 3f);
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                executioner.SwitchState(executioner.attackState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
            WalkAround();
        }
        void WalkAround()
        {
            Controller target = executioner.GetTargetsInView();
            executioner.moveAmount = 0;
            if (!target) return;
            Vector3 direction = (target.transform.position - executioner.transform.position);
            if (Vector3.SqrMagnitude(target.transform.position - controller.transform.position) <= 9 && Vector3.Dot(Vector3.right * controller.core.movement.facingDirection, direction) > 0)
            {
                return;
            }
            executioner.moveAmount = 1;
            executioner.HandleLockTarget(target.transform, 0.5f);
            executioner.core.movement.SetVelocityX(executioner.core.movement.facingDirection * 3);
        }
    }
}

