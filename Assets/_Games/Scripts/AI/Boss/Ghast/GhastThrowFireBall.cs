using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class GhastThrowFireBallState : State
    {
        private BossGhost ghast;
        private int fireBallCount;
        private float fireRate;
        private Transform animatorTransform;
        public GhastThrowFireBallState(BossGhost controller, string stateName) : base(controller, stateName)
        {
            ghast = controller;
            animatorTransform = controller.animatorHandle.transform;
        }

        public override void EnterState()
        {
            fireBallCount = Random.Range(5, 8);
        }

        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
            fireRate -= Time.deltaTime;
            if (fireRate <= 0)
            {
                fireBallCount--;
                fireRate = ghast.attackRate;
                Vector2 direction = ghast.player.transform.position - transform.position;
                ghast.ThrowFireBall(direction.normalized);
            }
            if (fireBallCount == 0)
            {
                ghast.SwitchState(ghast.roamingState);
                return;
            }
            Vector3 dir = ghast.player.transform.position - transform.position;
            HandleRotation(dir.x);
        }
        private void HandleRotation(float direction)
        {
            if (direction < 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
            }
            else if (direction > 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
            }
        }
        public override void UpdatePhysic()
        {

        }
    }
}
