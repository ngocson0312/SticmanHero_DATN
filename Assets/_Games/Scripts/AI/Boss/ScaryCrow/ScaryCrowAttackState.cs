using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ScaryCrowAttackState : State
    {
        private ScaryCrow scaryCrow;
        private float timer;
        public ScaryCrowAttackState(ScaryCrow controller, string stateName) : base(controller, stateName)
        {
            scaryCrow = controller;
        }

        public override void EnterState()
        {
            Collider2D[] colls = scaryCrow.GetTargetsInView();
            scaryCrow.moveAmount = 0;
            controller.core.movement.SetVelocityX(0);
            if (colls.Length == 0)
            {
                scaryCrow.SwitchState(scaryCrow.idleState);
                return;
            }
            Transform target = colls[0].transform;
            float distance = Vector3.SqrMagnitude(controller.transform.position - target.position);
            if (distance >= 25)
            {
                if (Random.Range(0, 100) < 50)
                {
                    scaryCrow.AddAction(new ScaryCrowChargeAction(scaryCrow, target));
                    scaryCrow.AddAction(new ScaryCrowChargeAction(scaryCrow, target));
                    timer = 6f;
                }
                else
                {
                    scaryCrow.SwitchState(scaryCrow.chaseState);
                    return;
                }
            }
            else
            {
                int r = Random.Range(0, 100);
                if (r < 20)
                {
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "SingleAttack", target, 1.5f));
                    scaryCrow.AddAction(new ScaryCrowChargeAction(scaryCrow, target));
                    timer = 5f;
                    Debug.Log("combo1");
                }
                else if (r >= 20 && r < 40)
                {
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "Combo", target, 2f));
                    scaryCrow.AddAction(new ScaryCrowChargeAction(scaryCrow, target));
                    timer = 6f;
                }
                else if (r >= 40 && r < 60)
                {
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "SingleAttack", target, 1.5f));
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "Combo", target, 2f));
                    scaryCrow.AddAction(new ScaryCrowChargeAction(scaryCrow, target));
                    timer = 6f;
                    Debug.Log("combo2");
                }
                else
                {
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "Combo", target, 2f));
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "SingleAttack", target, 1.5f));
                    scaryCrow.AddAction(new ScaryCrowAttackAction(scaryCrow, "SingleAttack", target, 1.5f));
                    timer = 5f;
                    Debug.Log("combo3");
                }
            }
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            Collider2D[] colls = scaryCrow.GetTargetsInView();
            if (colls.Length > 0 && scaryCrow.rotateWhenAction)
            {
                scaryCrow.HandleLockTarget(colls[0].transform, 0.5f);
            }
            if (timer <= 0)
            {
                scaryCrow.SwitchState(scaryCrow.idleState);
                return;
            }
        }

        public override void UpdatePhysic()
        {
        }
    }
}

