using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class HRTeleportAttackAction : AIAction
    {
        private HornetRequiem hornetRequiem;
        private Transform target;
        private float timer;
        private int state;
        private Tween tween;
        public HRTeleportAttackAction(HornetRequiem controller, Transform target) : base(controller)
        {
            hornetRequiem = controller;
            this.target = target;
        }

        public override void ExitAction()
        {
            hornetRequiem.FinishAction();
        }

        public override void OnActionEnter()
        {
            timer = 0;
            state = 0;
            hornetRequiem.animatorHandle.PlayAnimation("Dash", 0.1f, 1, true);
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
                hornetRequiem.HandleLockTarget(target, 0.5f);
                if (timer >= 1.5f)
                {
                    state = 1;
                    hornetRequiem.HandleLockTarget(target, 1);
                }
            }
            if (state == 1)
            {
                if (timer >= 2)
                {
                    state = 2;
                }
            }
            if (state == 2)
            {
                hornetRequiem.core.movement.SetVelocityX(0);
                Vector3 pos;
                if (hornetRequiem.core.movement.facingDirection > 0)
                {
                    pos = new Vector3(target.position.x - 3, hornetRequiem.transform.position.y, 0);
                }
                else
                {
                    pos = new Vector3(target.position.x + 3, hornetRequiem.transform.position.y, 0);
                }

                if (pos.x < hornetRequiem.xMin)
                {
                    pos.x = hornetRequiem.xMin;
                }
                if (pos.x > hornetRequiem.xMax)
                {
                    pos.x = hornetRequiem.xMax;
                }

                float time = Mathf.Abs(target.transform.position.x - hornetRequiem.transform.position.x) / 50f;
                if (time < 0.3f)
                {
                    time = 0.3f;
                }
                if (time > 1)
                {
                    time = 1;
                }
                hornetRequiem.transform.DOMoveX(pos.x, time).OnComplete(() =>
                {
                    hornetRequiem.HandleLockTarget(target, 1);
                    hornetRequiem.animatorHandle.PlayAnimation("Slash", 0.1f, 1, true);
                });
                state = 3;
            }
            if (state == 3 && timer > 3f)
            {
                ExitAction();
            }
        }

        public override void UpdatePhysic(float fixedDeltaTime)
        {
            if (state == 1)
            {
                hornetRequiem.core.movement.SetVelocityX(50 * hornetRequiem.core.movement.facingDirection);
                if (hornetRequiem.core.collisionSenses.IsTouchWall())
                {
                    hornetRequiem.core.movement.Flip();
                    if (hornetRequiem.core.movement.facingDirection < 0)
                    {
                        hornetRequiem.animatorHandle.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    }
                    if (hornetRequiem.core.movement.facingDirection > 0)
                    {
                        hornetRequiem.animatorHandle.transform.localRotation = Quaternion.Euler(0, 100, 0);
                    }
                }
            }
        }
    }
}
