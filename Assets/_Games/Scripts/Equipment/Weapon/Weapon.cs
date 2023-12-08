using System;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public abstract class Weapon : Equipment
    {
        public MoveSet[] moveSets;
        public float attackRange;
        public LayerMask layerContact;
        public bool isTrigger;
        public bool isActiveCombo;
        protected Controller controller;
        protected List<StatusEffectData> listEffect;
        public override void Initialize(Controller controller)
        {
            this.controller = controller;
            equipmentType = EquipmentType.WEAPON;
            listEffect = new List<StatusEffectData>();
        }
        public override void SetEquipmentData(EquipmentData data)
        {
            base.SetEquipmentData(data);
            int level = data.level - 1;
            switch (data.grade)
            {
                case 1:
                    equipmentStats.damage = 10 + level * 3;
                    break;
                case 2:
                    equipmentStats.damage = 20 + level * 5;
                    break;
                case 3:
                    equipmentStats.damage = 30 + level * 7;
                    break;
                case 4:
                    equipmentStats.damage = 50 + level * 9;
                    break;
                case 5:
                    equipmentStats.damage = 70 + level * 12;
                    break;
                case 6:
                    equipmentStats.damage = 100 + level * 15;
                    break;
            }
        }
        public abstract void TriggerWeapon();
        public abstract void ResetAnimation();
        public virtual void SetActive(bool status)
        {
            isTrigger = status;
            gameObject.SetActive(status);
        }
        public virtual void OnEquip()
        {
            controller.animatorHandle.OnEventAnimation += OnEvent;
        }
        public virtual void OnUnEquip()
        {
            controller.animatorHandle.OnEventAnimation -= OnEvent;
        }
        public abstract float GetDurability();
        public virtual float GetReloadNormalizedTime()
        {
            return 0;
        }
        protected abstract void OnEvent(string eventName);
    }
    [System.Serializable]
    public class MoveSet
    {
        public string animationName;
        public int layer;
        public float stunTime;
        public Vector2 force;
        public AudioClip activeSound;
        public AudioClip impactSound;
    }
}

