using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public abstract class Equipment : MonoBehaviour
    {
        public CharacterStats equipmentStats;
        public EquipmentType equipmentType;
        public EquipmentData data;
        public abstract void Initialize(Controller controller);
        public virtual void SetEquipmentData(EquipmentData data)
        {
            equipmentStats = new CharacterStats();
            this.data = data;
        }
        public abstract void SetUpPassive(CharacterStats originalStats);
        public abstract void OnUpdate();
    }
    public enum EquipmentType
    {
        WEAPON, ARMOR, BOOTS, NECKLACE, RING, SKIN
    }
}
