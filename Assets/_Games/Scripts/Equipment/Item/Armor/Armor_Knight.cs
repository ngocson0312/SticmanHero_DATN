using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Armor_Knight : Armor
    {
        private int passiveData;
        private bool isPassiveActivated;
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
                    equipmentStats.armor += 20;
                    break;
                case 4:
                    equipmentStats.armor += 20;
                    equipmentStats.armor += originalStats.armor * 10 / 100;
                    break;
                case 5:
                    equipmentStats.armor += 20;
                    equipmentStats.armor += originalStats.armor * 10 / 100;
                    break;
                case 6:
                    equipmentStats.armor += 20;
                    equipmentStats.armor += originalStats.armor * 10 / 100;
                    equipmentStats.health += originalStats.health * 10 / 100;
                    break;
            }
            passiveData = controller.originalStats.armor * 30 / 100;
        }
        public override void OnEquip()
        {
            base.OnEquip();
        }
        public override void OnUnEquip()
        {
            base.OnUnEquip();
        }

        public override void OnUpdate()
        {
            if (data.grade >= 5) // Passive: duoi 50% HP tang 30% armor
            {
                if (controller.runtimeStats.health <= controller.originalStats.health / 2)
                {
                    if (!isPassiveActivated)
                    {
                        isPassiveActivated = true;
                        controller.runtimeStats.armor += passiveData;
                    }
                }
                else
                {
                    if (isPassiveActivated)
                    {
                        isPassiveActivated = false;
                        controller.runtimeStats.armor -= passiveData;
                    }
                }
            }
        }
    }
}
