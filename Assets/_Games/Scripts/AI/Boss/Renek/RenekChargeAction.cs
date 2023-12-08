using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekChargeAction : AIAction
    {
        private Renek renek;
        private Transform target;
        private int state;
        private float timer;
        public RenekChargeAction(Renek controller, Transform target) : base(controller)
        {
            renek = controller;
            this.target = target;
        }

        public override void ExitAction()
        {
            renek.FinishAction();
        }

        public override void OnActionEnter()
        {
            renek.animatorHandle.PlayAnimation("StartCharging", 0.1f, 1, true);
            renek.renekAnimator.StartCharging();
            state = 0;
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
            if (timer <= 0.45f)
            {
                renek.HandleLockTarget(target, 1f);
            }
            if (timer >= 0.55f && state == 0)
            {
                controller.core.movement.SetVelocityX(15 * controller.core.movement.facingDirection);
                renek.renekAnimator.ChargeCollider();
            }
            if (timer >= 1f && state == 0)
            {
                // renek.HandleLockTarget(target);
                renek.animatorHandle.PlayAnimation("FinishCharging", 0.1f, 1, true);
                state = 1;
            }
            if (timer >= 2f && state == 1)
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
