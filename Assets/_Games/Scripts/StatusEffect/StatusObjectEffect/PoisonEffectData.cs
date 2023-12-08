using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PoisonEffectData : StatusEffectData
    {
        public int rawDamage;
        public int damagePerHP;
        public PoisonEffectData(Controller owner, StatusEffectType effectType) : base(owner, effectType)
        {
            rawDamage = 3;
            damagePerHP = 1;
        }
    }
}
