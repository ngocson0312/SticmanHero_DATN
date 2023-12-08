using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ElectrocuteEffect : StatusEffect
    {
        [SerializeField] float affectedTime;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            statusName = StatusEffectType.ELECTROCUTE;
            affectedTime = 1f;
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
            affectedTime -= Time.deltaTime;
            if (affectedTime < 0)
            {
                OnFinishEffect();
                return;
            }
        }
    }
}
