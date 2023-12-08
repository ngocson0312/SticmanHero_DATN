using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class StatusEffectManager : Singleton<StatusEffectManager>
    {
        [SerializeField] BurningEffect burningEffect;
        [SerializeField] FrostbiteEffect frostbiteEffect;
        [SerializeField] ElectrocuteEffect ElectrocuteEffect;
        [SerializeField] BleedingEffect bleedingEffect;
        [SerializeField] PoisonEffect poisonEffect;
        [SerializeField] CurseEffect curseEffect;
        public StatusEffect CreateEffect(StatusEffectData statusEffectData)
        {
            StatusEffect currentEffect = null;
            switch (statusEffectData.effectType)
            {
                case StatusEffectType.BURNING:
                    currentEffect = burningEffect;
                    break;
                case StatusEffectType.FROSTBITE:
                    currentEffect = frostbiteEffect;
                    break;
                case StatusEffectType.ELECTROCUTE:
                    currentEffect = ElectrocuteEffect;
                    break;
                case StatusEffectType.BLEEDING:
                    currentEffect = bleedingEffect;
                    break;
                case StatusEffectType.POISON:
                    currentEffect = poisonEffect;
                    break;
                case StatusEffectType.CURSE:
                    currentEffect = curseEffect;
                    break;

            }
            StatusEffect statusEffect = Instantiate(currentEffect);
            statusEffect.Initialize(statusEffectData);
            return statusEffect;
        }
    }
}
