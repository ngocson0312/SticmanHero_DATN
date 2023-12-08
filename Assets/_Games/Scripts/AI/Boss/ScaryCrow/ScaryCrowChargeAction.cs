using UnityEngine;
namespace SuperFight
{
    public class ScaryCrowChargeAction : AIAction
    {
        private ScaryCrow scaryCrow;
        private int state;
        private float timer;
        private Transform target;
        float dealDamageTimer;
        public ScaryCrowChargeAction(ScaryCrow controller, Transform target) : base(controller)
        {
            scaryCrow = controller;
            this.target = target;
        }

        public override void ExitAction()
        {
            scaryCrow.FinishAction();
        }

        public override void OnActionEnter()
        {
            scaryCrow.animatorHandle.PlayAnimation("RunAttack", 0.1f, 1, true);
            state = 0;
            timer = 0;
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
                scaryCrow.HandleLockTarget(target, 0.5f);
            }
            if (timer >= 0.5f && state == 0)
            {
                controller.core.movement.SetVelocityX(10 * controller.core.movement.facingDirection);
                dealDamageTimer -= deltaTime;
                if (dealDamageTimer <= 0)
                {
                    scaryCrow.scaryCrowAnimator.ChargeCollider();
                    dealDamageTimer = 0.2f;
                }
            }
            if (timer >= 2f && state == 0)
            {
                scaryCrow.HandleLockTarget(target, 1f);
                controller.core.movement.SetVelocityX(0);
                scaryCrow.animatorHandle.PlayAnimation("RunAttack_3", 0.1f, 1, false);
                state = 1;
            }
            if (timer >= 3f && state == 1)
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

