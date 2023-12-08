using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class BleedingEffectData : StatusEffectData
    {
        public int rawDamage;
        public int damagePerHP;
        public BleedingEffectData(Controller owner, StatusEffectType effectType) : base(owner, effectType)
        {
            rawDamage = 5;
            damagePerHP = 10;
        }
    }
}
