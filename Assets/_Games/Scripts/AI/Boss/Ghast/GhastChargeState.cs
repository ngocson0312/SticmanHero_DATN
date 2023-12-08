using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class GhastChargeState : State
    {
        private BossGhost ghast;
        private float timer;
        private int state;
        private Vector3 targetPosition;
        public GhastChargeState(BossGhost controller, string stateName) : base(controller, stateName)
        {
            ghast = controller;
        }

        public override void EnterState()
        {
            // ghast.headDamage.ResetCollider();
            timer = 2f;
            state = 0;
            ghast.chargeFX.Play();
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            ghast.animatorHandle.transform.Rotate(new Vector3(0, 10, 0));
            if (state == 0)
            {
                if (timer <= 0)
                {
                    targetPosition = ghast.player.transform.position + Vector3.up * 1;
                    state = 1;
                    timer = 0.5f;
                    ghast.lineWarning.enabled = true;
                    ghast.lineWarning.SetPosition(0, transform.position);
                    ghast.lineWarning.SetPosition(1, targetPosition);
                    ghast.chargeFX.Stop();
                }
            }
            if (state == 1)
            {
                if (timer <= 0)
                {
                    state = 2;
                    ghast.lineWarning.enabled = false;
                }
            }
            if (state == 2)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, 50 * Time.deltaTime);
                if (transform.position == targetPosition)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = ghast.runtimeStats.damage;
                    damageInfo.characterType = controller.characterType;
                    damageInfo.owner = controller;
                    damageInfo.listEffect = new List<StatusEffectData>();
                    damageInfo.listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                    if (Vector2.Distance(transform.position, ghast.player.transform.position) <= 4)
                    {
                        ghast.player.playerController.OnTakeDamage(damageInfo);
                    }
                    // ghast.headDamage.SetActive(true);
                    // ghast.headDamage.SetDamageData(damageInfo);
                    ghast.explosionFX.Play();
                    ghast.stunFX.Play();
                    state = 3;
                    timer = 3f;
                }
            }

            if (state == 3)
            {
                if (timer <= 0)
                {
                    ghast.stunFX.Stop();
                    ghast.SwitchState(ghast.roamingState);
                }
            }
        }

        public override void UpdatePhysic()
        {

        }
    }
}
