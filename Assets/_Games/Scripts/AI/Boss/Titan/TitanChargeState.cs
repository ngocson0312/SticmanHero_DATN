using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class TitanChargeState : State
    {
        BossTitan titan;
        int chargeTime;
        int chargeCount;
        float timer;
        int chargeState;
        public TitanChargeState(BossTitan controller, string stateName) : base(controller, stateName)
        {
            titan = controller;
        }

        public override void EnterState()
        {
            chargeCount = 0;
            timer = 0;
            chargeState = 1;
            chargeTime = 3;
        }

        public override void ExitState()
        {
        }

        public override void UpdateLogic()
        {
            HandleCharge();
        }

        public override void UpdatePhysic()
        {

        }
        void HandleCharge()
        {
            timer -= Time.deltaTime;
            if (chargeState == 1)
            {
                int direction = core.movement.facingDirection;
                core.movement.SetVelocityX(direction * titan.speed);
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = titan.runtimeStats.damage;
                damageInfo.owner = titan;
                damageInfo.stunTime = 0.5f;
                damageInfo.hitDirection = direction;

                titan.headDamage.SetActive(true);
                titan.headDamage.SetDamageData(damageInfo);

                if (core.collisionSenses.IsTouchWall())
                {
                    CameraController.Instance.ShakeCamera(.35f, 1f, 10, 90, true);
                    titan.FXBossTitanDir.Play();
                    chargeCount++;
                    titan.headDamage.SetActive(false);
                    titan.headDamage.ResetCollider();
                    if (chargeCount >= chargeTime)
                    {
                        titan.stunFX.SetActive(true);
                        titan.animatorHandle.PlayAnimation("Stun", 0.1f, 1, false);
                        timer = 5f;
                        chargeState = 3;
                        chargeCount = 0;
                    }
                    else
                    {
                        timer = Random.Range(0.2f, 1f);
                        chargeState = 2;
                    }
                }
                titan.animatorHandle.SetFloat("MoveAmount", 2);
            }
            if (chargeState == 2)
            {
                titan.animatorHandle.SetFloat("MoveAmount", 0);
                if (timer <= 0)
                {
                    core.movement.Flip();
                    chargeState = 1;
                }
            }

            if (chargeState == 3)
            {
                titan.animatorHandle.SetFloat("MoveAmount", 0);
                if (timer <= 0)
                {
                    titan.stunFX.SetActive(false);
                    core.movement.Flip();
                    chargeState = 1;
                }
            }

            if (chargeState != 3)
            {
                Transform target = titan.animatorHandle.transform;
                if (core.movement.facingDirection < 0)
                {
                    target.localRotation = Quaternion.Lerp(target.localRotation, Quaternion.Euler(0, 250, 0), 0.2f);
                }
                else if (core.movement.facingDirection > 0)
                {
                    target.localRotation = Quaternion.Lerp(target.localRotation, Quaternion.Euler(0, 110, 0), 0.2f);
                }
            }
        }
    }
}
