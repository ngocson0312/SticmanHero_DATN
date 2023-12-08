using System.Collections;
using System;
using UnityEngine;

namespace SuperFight
{
    public abstract class StatusEffect : MonoBehaviour
    {
        protected Controller controller;
        protected Controller owner;
        public Action OnComplete;
        public StatusEffectType statusName;
        public Transform iconEffect;
        public virtual void Initialize(StatusEffectData statusEffectData)
        {
            owner = statusEffectData.owner;
        }
        public virtual void OnStartEffect(Controller controller)
        {
            this.controller = controller;
        }
        public abstract void UpdateEffect();
        public abstract void OnFinishEffect();
    }
    public enum StatusEffectType
    {
        BURNING, FROSTBITE, ELECTROCUTE, BLEEDING, POISON, CURSE
    }
}

