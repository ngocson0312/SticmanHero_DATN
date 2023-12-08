using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HRComboAction : AIAction
    {
        private float timer;
        private HornetRequiem hornetRequiem;
        private Transform target;
        public HRComboAction(HornetRequiem controller, Transform target, float duration) : base(controller)
        {
            hornetRequiem = controller;
            this.target = target;
            timer = duration;
        }

        public override void ExitAction()
        {
            hornetRequiem.FinishAction();
        }

        public override void OnActionEnter()
        {
            hornetRequiem.HandleLockTarget(target, 1f);
            controller.animatorHandle.PlayAnimation("Combo", 0.1f, 1, true);
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

