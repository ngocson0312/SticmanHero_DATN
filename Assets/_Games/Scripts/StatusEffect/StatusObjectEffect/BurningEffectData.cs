using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class BurningEffectData : StatusEffectData
    {
        public int damagePerHP;
        public int damageRaw;
        public BurningEffectData(Controller owner, StatusEffectType effectType) : base(owner, effectType)
        {
            damagePerHP = 2;
            damageRaw = 10;
        }
    }
}
