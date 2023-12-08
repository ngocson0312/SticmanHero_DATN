using System.Collections.Generic;
using DG.Tweening;
using mygame.sdk;
using UnityEngine;
namespace SuperFight
{
    public class CreeperExplodeState : State
    {
        Creeper creeper;
        float explodeTimer;
        bool animExplosion;
        public CreeperExplodeState(Creeper controller, string stateName) : base(controller, stateName)
        {
            creeper = controller;
            creeper.animatorHandle.OnEventAnimation += CriticalEvent;
        }
        ~CreeperExplodeState()
        {

            creeper.animatorHandle.OnEventAnimation -= CriticalEvent;
        }
        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                creeper.CiticalVfx.Play();
            }

        }

        public override void EnterState()
        {
            controller.core.movement.SetVelocityX(0);
            AudioManager.Instance.PlayOneShot(creeper.countDownSfx, 1);
            explodeTimer = 1f;
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            if (!creeper.isActive) return;
            if (explodeTimer > 0)
            {
                if (!animExplosion)
                {
                    animExplosion = true;
                    creeper.animatorHandle.PlayAnimation("Attack", 0.1f, 1, false);
                    creeper.rendererCharacter.material.DOColor(Color.red, 1.5f);
                }
                explodeTimer -= Time.deltaTime;
                if (explodeTimer <= 0)
                {
                    animExplosion = false;
                    AudioManager.Instance.PlayOneShot(creeper.explosionSfx, 1f);
                    creeper.VfXExpolision.Play();
                    GameHelper.Instance.Vibrate(Type_vibreate.Vib_Light);
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.characterType = CharacterType.Mob;
                    damageInfo.damage = controller.runtimeStats.damage;
                    damageInfo.stunTime = 0.1f;
                    damageInfo.listEffect = new List<StatusEffectData>();
                    damageInfo.listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                    Bounds bounds = new Bounds();
                    bounds.center = transform.position;
                    bounds.size = new Vector3(4f, 4f);
                    Controller target = creeper.GetTargetInView(bounds);
                    if (target != null && Vector2.Distance(target.transform.position, creeper.transform.position) <= 4)
                    {
                        target.OnTakeDamage(damageInfo);
                    }
                    creeper.Die(true);
                    CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
                }
            }

        }

        public override void UpdatePhysic()
        {
        }
    }

}
