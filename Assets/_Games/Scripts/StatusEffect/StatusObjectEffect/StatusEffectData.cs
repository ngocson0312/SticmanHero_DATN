using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public abstract class StatusEffectData
    {
        public StatusEffectType effectType;
        public Controller owner;
        public StatusEffectData(Controller owner, StatusEffectType effectType)
        {
            this.owner = owner;
            this.effectType = effectType;
        }
    }
}
