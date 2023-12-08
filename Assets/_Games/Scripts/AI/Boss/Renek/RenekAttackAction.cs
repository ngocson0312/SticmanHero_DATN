using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekAttackAction : AIAction
    {
        private float timer;
        private Renek renek;
        private string animationName;
        public RenekAttackAction(Renek controller, string animationName, float actionTime) : base(controller)
        {
            renek = controller;
            timer = actionTime;
            this.animationName = animationName;
        }

        public override void ExitAction()
        {
            renek.FinishAction();
        }

        public override void OnActionEnter()
        {
            renek.animatorHandle.PlayAnimation(animationName, 0.1f, 1, true);
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
