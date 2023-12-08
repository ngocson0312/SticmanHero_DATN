using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public abstract class Boots : Equipment
    {
        protected Controller controller;
        public override void Initialize(Controller controller)
        {
            this.controller = controller;
            equipmentType = EquipmentType.BOOTS;

        }
        public override void SetEquipmentData(EquipmentData data)
        {
            base.SetEquipmentData(data);
            int level = data.level - 1;
            switch (data.grade)
            {
                case 1:
                    equipmentStats.armor = 5 + level * 1;
                    break;
                case 2:
                    equipmentStats.armor = 10 + level * 2;
                    break;
                case 3:
                    equipmentStats.armor = 15 + level * 3;
                    break;
                case 4:
                    equipmentStats.armor = 20 + level * 4;
                    break;
                case 5:
                    equipmentStats.armor = 30 + level * 5;
                    break;
                case 6:
                    equipmentStats.armor = 50 + level * 7;
                    break;
            }
        }
        public virtual void OnEquip()
        {
            // controller.originalStats.armor += armorBonus;
        }
        public virtual void OnUnEquip()
        {
            // controller.originalStats.armor -= armorBonus;
        }
    }
}
