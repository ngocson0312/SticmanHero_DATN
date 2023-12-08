using System;
using UnityEngine;
namespace SuperFight
{
    public class BurningEffect : StatusEffect
    {
        private float timer;
        public float duration;
        private int rawDamage;
        private int damagePerHP;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            statusName = StatusEffectType.BURNING;
            BurningEffectData burningEffectData = statusEffectData as BurningEffectData;
            rawDamage = burningEffectData.damageRaw;
            damagePerHP = burningEffectData.damagePerHP;
            timer = 0.5f;
        }
        public override void OnStartEffect(Controller controller)
        {
            base.OnStartEffect(controller);
            duration = 1.5f;
        }
        public override void OnFinishEffect()
        {
            Destroy(gameObject);
            OnComplete?.Invoke();
        }
        public override void UpdateEffect()
        {
            duration -= Time.deltaTime;
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = (int)(rawDamage + (controller.originalStats.health * damagePerHP / 100f));
                damageInfo.owner = owner;
                damageInfo.damageType = DamageType.TRUEDAMAGE;
                controller.OnTakeDamage(damageInfo);
                timer = 0.5f;
            }
            if (duration <= 0)
            {
                OnFinishEffect();
                return;
            }
        }
    }
}
