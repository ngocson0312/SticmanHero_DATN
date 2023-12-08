using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SuperFight
{
    public class BleedingEffect : StatusEffect
    {
        [SerializeField] float bleedingTime;
        [SerializeField] float duration;
        [SerializeField] ParticleSystem bleedingEff;
        [SerializeField] ParticleSystem bleedingExplosionEff;
        // public int bleedingPoint;
        private int rawDamage;
        private int damagePerHP;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            BleedingEffectData bleedingEffect = statusEffectData as BleedingEffectData;
            statusName = StatusEffectType.BLEEDING;
            rawDamage = bleedingEffect.rawDamage;
            damagePerHP = bleedingEffect.damagePerHP;
            bleedingTime = 3f;
            duration = .3f;
        }
        public override void OnStartEffect(Controller controller)
        {
            base.OnStartEffect(controller);
        }

        public void BleedingTarget()
        {
            bleedingEff.Stop();
            bleedingExplosionEff.Play();
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = (int)(controller.originalStats.health * damagePerHP / 100f);
            damageInfo.damageType = DamageType.TRUEDAMAGE;
            damageInfo.owner = owner;
            controller.OnTakeDamage(damageInfo);
            OnFinishEffect();
        }
        public override void OnFinishEffect()
        {
            Destroy(gameObject, 1f);
            OnComplete?.Invoke();
        }
        public override void UpdateEffect()
        {
            bleedingTime -= Time.deltaTime;
            if (bleedingTime > 0)
            {
                duration -= Time.deltaTime;
                if (duration < 0f)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = rawDamage;
                    damageInfo.owner = owner;
                    controller.OnTakeDamage(damageInfo);
                    duration = .3f;
                }
            }
            else
            {
                OnFinishEffect();
            }
            // if (bleedingPoint >= 4)
            // {
            //     bleedingIconEff.gameObject.SetActive(true);
            // }
            // if (bleedingPoint >= 5)
            // {
            //     BleedingTarget();
            // }
        }
    }
}

