using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class GhastRoamingState : State
    {
        private BossGhost ghast;
        private float roamingTimer;
        private int indexPos;
        private float idleTimer;
        int maxAttackInTurn;
        private Transform animatorTransform;
        public GhastRoamingState(BossGhost controller, string stateName) : base(controller, stateName)
        {
            this.ghast = controller;
            animatorTransform = controller.animatorHandle.transform;
            indexPos = 0;
        }

        public override void EnterState()
        {
            
            maxAttackInTurn = Random.Range(5, 7);
            idleTimer = 1f;
        }

        public override void ExitState()
        {
            indexPos++;
            if (indexPos >= ghast.posMoves.Length)
            {
                indexPos = 0;
            }
        }

        public override void UpdateLogic()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer > 0) return;
            HandleRoaming();
        }

        private void HandleRoaming()
        {
            Vector3 direction = ghast.posMoves[indexPos].position - transform.position;
            direction.z = 0;
            transform.position += direction.normalized * ghast.flySpeed * Time.deltaTime;
            HandleRotation(direction.x);
            if (direction.magnitude <= 1)
            {
                ghast.stunFX.Stop();
                int rand = Random.Range(0, 100);
                if (rand < 30)
                {
                    ghast.SwitchState(ghast.throwFireBall);
                }
                else if (rand > 30 && rand < 60)
                {
                    ghast.SwitchState(ghast.laserState);
                }
                else
                {
                    ghast.SwitchState(ghast.ghastChargeState);
                }
                return;
            }
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
