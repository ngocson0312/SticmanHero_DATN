using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ScaryCrowAttackAction : AIAction
    {
        private ScaryCrow scaryCrow;
        private float timer;
        private string amimationName;
        private Transform target;
        public ScaryCrowAttackAction(ScaryCrow controller, string amimationName, Transform target, float actionTime) : base(controller)
        {
            scaryCrow = controller;
            this.amimationName = amimationName;
            timer = actionTime;
            this.target = target;
        }
        public override void ExitAction()
        {
            scaryCrow.FinishAction();
        }

        public override void OnActionEnter()
        {
            scaryCrow.HandleLockTarget(target, 1f);
            scaryCrow.animatorHandle.PlayAnimation(amimationName, 0f, 1, true);
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }

        public override void UpdateLogic(float deltaTime)
        {
            timer -= deltaTime;
            if (timer <= 0)
            {
                ExitAction();
            }
        }

        public override void UpdatePhysic(float fixedDeltaTime)
        {
        }
    }

}
