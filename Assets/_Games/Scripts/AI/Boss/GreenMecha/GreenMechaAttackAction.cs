using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMechaAttackAction : AIAction
    {
        private GreenMecha greenMecha;
        private float timer;
        private string animationName;
        public GreenMechaAttackAction(GreenMecha controller, string animationName, float duration) : base(controller)
        {
            greenMecha = controller;
            timer = duration;
            this.animationName = animationName;
        }

        public override void ExitAction()
        {
            greenMecha.FinishAction();
        }

        public override void OnActionEnter()
        {
            controller.animatorHandle.PlayAnimation(animationName, 0.1f, 1, true);
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

