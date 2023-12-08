using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GroundSpikeAction : AIAction
    {
        private GreenMecha greenMecha;
        private float timer;
        private int state;
        private Transform target;
        public GroundSpikeAction(GreenMecha controller, Transform target) : base(controller)
        {
            this.target = target;
            greenMecha = controller;
        }

        public override void ExitAction()
        {
            greenMecha.FinishAction();
        }

        public override void OnActionEnter()
        {
            timer = 0;
            state = 0;
            controller.animatorHandle.PlayAnimation("Raise", 0.1f, 1, true);
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }

        public override void UpdateLogic(float deltaTime)
        {
            timer += deltaTime;
            if (state == 0)
            {
                if (timer <= 0.8f)
                {
                    greenMecha.HandleLockTarget(target, 1);
                }
                if (timer >= 1)
                {
                    controller.animatorHandle.PlayAnimation("Throw", 0.1f, 1, true);
                    state = 1;
                }
            }
            if (state == 1 && timer >= 2f)
            {
                ExitAction();
                state = 2;
            }
        }

        public override void UpdatePhysic(float fixedDeltaTime)
        {
        }
    }
}

