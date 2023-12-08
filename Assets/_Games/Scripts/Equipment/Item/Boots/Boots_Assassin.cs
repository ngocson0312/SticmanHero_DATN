using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{

    public class Boots_Assassin : Boots
    {
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
            equipmentStats = new CharacterStats();
            switch (data.grade)
            {
                case 3:
                    equipmentStats.armor += 10;
                    break;
                case 4:
                    equipmentStats.armor += 10;
                    equipmentStats.armor += originalStats.armor * 5 / 100;
                    break;
                case 5:
                    equipmentStats.armor += 10;
                    equipmentStats.armor += originalStats.armor * 5 / 100;
                    break;
                case 6:
                    equipmentStats.armor += 10;
                    equipmentStats.armor += originalStats.armor * 5 / 100;
                    equipmentStats.health += originalStats.health * 5 / 100;
                    break;
            }
        }
        public override void OnEquip()
        {
            controller.OnCrit += OnCrit;
            base.OnEquip();
        }

        private void OnCrit()
        {
            if (data.grade >= 5)
            {
                controller.AddHealth(50);
            }
        }

        public override void OnUnEquip()
        {
            controller.OnCrit -= OnCrit;
            base.OnUnEquip();
        }

        public override void OnUpdate()
        {
        }
    }
}
