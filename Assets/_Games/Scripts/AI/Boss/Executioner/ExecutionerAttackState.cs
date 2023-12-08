using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ExecutionerAttackState : State
    {
        Executioner executioner;
        float timeRecover;
        public ExecutionerAttackState(Executioner controller, string stateName) : base(controller, stateName)
        {
            executioner = controller;
        }

        public override void EnterState()
        {
            Controller targetX = executioner.GetTargetsInView();
            if (!targetX)
            {
                executioner.SwitchState(executioner.idleState);
                return;
            }
            Transform target = targetX.transform;
            float distance = Vector3.SqrMagnitude(controller.transform.position - target.position);
            executioner.HandleLockTarget(target, 1f);
            if (distance > 16)
            {
                executioner.AddAction(new JumpingAttackAction(executioner, target, 0));
                executioner.AddAction(new ThrowAxeAction(executioner, target, 0.5f));
                timeRecover = 4f;
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r < 30)
                {
                    executioner.AddAction(new ExecutionerComboAction(executioner, target, 0f));
                    executioner.AddAction(new JumpingAttackAction(executioner, target, 0));
                    timeRecover = 4f;
                }
                else if (r > 70)
                {
                    executioner.AddAction(new ExecutionerComboAction(executioner, target, 0));
                    executioner.AddAction(new ThrowAxeAction(executioner, target, 0.5f));
                    executioner.AddAction(new JumpingAttackAction(executioner, target, 0));
                    timeRecover = 5f;
                }
                else
                {
                    executioner.AddAction(new ThrowAxeAction(executioner, target, 0.5f));
                    executioner.AddAction(new ExecutionerComboAction(executioner, target, 0));
                    executioner.AddAction(new ThrowAxeAction(executioner, target, 0.5f));
                    timeRecover = 5f;
                }
            }

        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            timeRecover -= Time.deltaTime;
            Controller target = executioner.GetTargetsInView();
            if (target && executioner.rotateWhenAction)
            {
                executioner.HandleLockTarget(target.transform, 1f);
            }
            if (timeRecover <= 0)
            {
                executioner.SwitchState(executioner.idleState);
                return;
            }
        }

        public override void UpdatePhysic()
        {

        }
    }
}

