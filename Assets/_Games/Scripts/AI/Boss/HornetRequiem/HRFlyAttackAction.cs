using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class HRFlyAttackAction : AIAction
    {
        private HornetRequiem hornetRequiem;
        private Transform target;
        private float timer;
        private int state;
        private float originY;
        private Tween tween;
        public HRFlyAttackAction(HornetRequiem controller, Transform target) : base(controller)
        {
            hornetRequiem = controller;
            this.target = target;
        }

        public override void ExitAction()
        {
            hornetRequiem.EnableGravity();
            hornetRequiem.FinishAction();
        }

        public override void OnActionEnter()
        {
            timer = 0;
            state = 0;
            originY = hornetRequiem.transform.position.y;
            hornetRequiem.animatorHandle.PlayAnimation("StartFly", 0.1f, 1, true);
            hornetRequiem.DisableGravity();
        }

        public override void OnPause()
        {
            if (tween != null)
            {
                tween.Pause();
            }
        }

        public override void OnResume()
        {
            if (tween != null)
            {
                tween.Play();
            }
        }

        public override void UpdateLogic(float deltaTime)
        {
            timer += deltaTime;
            if (state == 0)
            {
                hornetRequiem.HandleLockTarget(target, 0.5f);
                if (timer >= 0.25f)
                {
                    tween = hornetRequiem.transform.DOMoveY(originY + 6f, 1f).SetEase(Ease.OutCubic);
                    state = 1;
                }
            }
            if (state == 1)
            {
                hornetRequiem.HandleLockTarget(target, 0.5f);
                if (timer >= 2f)
                {
                    hornetRequiem.HandleLockTarget(target, 1f);
                    hornetRequiem.animatorHandle.PlayAnimation("JumpAttack1", 0.1f, 1, true);
                    state = 2;
                }
            }
            if (state == 2 && timer >= 2.2f)
            {
                int currentDirection = hornetRequiem.core.movement.facingDirection;
                Vector3 pos = new Vector3(target.position.x, originY, 0);
                if (currentDirection > 0)
                {
                    pos.x -= 3;
                }
                else
                {
                    pos.x += 3;
                }

                if (pos.x < hornetRequiem.xMin)
                {
                    pos.x = hornetRequiem.xMin;
                }
                if (pos.x > hornetRequiem.xMax)
                {
                    pos.x = hornetRequiem.xMax;
                }
                tween = hornetRequiem.transform.DOMove(pos, 0.5f).SetEase(Ease.InExpo);
                state = 3;
            }
            if (state == 3)
            {
                if (timer >= 2.5f)
                {
                    hornetRequiem.animatorHandle.PlayAnimation("JumpAttack2", 0.1f, 1, true);
                    state = 4;
                }
            }
            if (state == 4 && timer >= 4.5f)
            {
                hornetRequiem.animatorHandle.PlayAnimation("JumpAttack3", 0.1f, 1, true);
                state = 5;
            }
            if (state == 5 && timer >= 5)
            {
                ExitAction();
            }
        }

        public override void UpdatePhysic(float fixedDeltaTime)
        {
        }
    }
}

