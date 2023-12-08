using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class CurseEffect : StatusEffect
    {

        public int cursePointAdd;
        [SerializeField] int cursePointMAX;
        [SerializeField] ParticleSystem curseEff;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            statusName = StatusEffectType.CURSE;
            cursePointAdd = 0;
            cursePointMAX = 5;
        }
        public override void OnStartEffect(Controller controller)
        {
            base.OnStartEffect(controller);
        }
        public override void OnFinishEffect()
        {
            Destroy(gameObject);
            OnComplete?.Invoke();
        }
        public override void UpdateEffect()
        {
            if(cursePointAdd >= cursePointMAX - 1)
            {
                curseEff.gameObject.SetActive(true);
            }
            if(cursePointAdd >= cursePointMAX)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = controller.runtimeStats.health;
                damageInfo.owner = owner;
                controller.OnTakeDamage(damageInfo);
                OnFinishEffect();

            }
        }
    }
}
