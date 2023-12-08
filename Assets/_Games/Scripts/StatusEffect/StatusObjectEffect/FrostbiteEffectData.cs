using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class FrostbiteEffectData : StatusEffectData
    {
        public int freezePoint;
        public FrostbiteEffectData(Controller owner, StatusEffectType effectType) : base(owner, effectType)
        {
            freezePoint = 20;
        }
    }
}
