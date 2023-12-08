using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PoisonEffect : StatusEffect
    {
        [SerializeField] float poisonedTime;
        [SerializeField] float duration;
        private int damageDecrease;
        private int rawDamage;
        private int damagePerHP;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            statusName = StatusEffectType.POISON;
            duration = 1f;
            poisonedTime = 10f;
            PoisonEffectData poisonEffect = statusEffectData as PoisonEffectData;
            rawDamage = poisonEffect.rawDamage;
            damagePerHP = poisonEffect.damagePerHP;
            damageDecrease = 0;
        }
        public override void OnStartEffect(Controller controller)
        {
            if (damageDecrease == 0)
            {
                damageDecrease = (int)(controller.originalStats.damage * 10f / 100f);
                controller.runtimeStats.damage -= damageDecrease;
            }
            poisonedTime = 10f;
            base.OnStartEffect(controller);
        }
        public override void OnFinishEffect()
        {
            controller.runtimeStats.damage += damageDecrease;
            Destroy(gameObject);
            OnComplete?.Invoke();
        }
        public override void UpdateEffect()
        {
            poisonedTime -= Time.deltaTime;
            if (poisonedTime > 0)
            {
                duration -= Time.deltaTime;
                if (duration < 0)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = rawDamage + (int)(controller.originalStats.health * damagePerHP / 100f);
                    damageInfo.damageType = DamageType.TRUEDAMAGE;
                    damageInfo.owner = owner;
                    controller.OnTakeDamage(damageInfo);
                    duration = 1f;
                }
            }
            if (poisonedTime < 0f)
            {
                OnFinishEffect();
            }
        }
    }
}

