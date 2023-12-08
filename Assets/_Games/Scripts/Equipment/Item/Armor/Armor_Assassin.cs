using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Armor_Assassin : Armor
    {
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
            if (data.grade >= 5) // Passive: tren 80% HP tang 30% crit dmg
            {
                if (controller.runtimeStats.health >= controller.originalStats.health * 0.8f)
                {
                    if (!isPassiveActivated)
                    {
                        isPassiveActivated = true;
                        controller.runtimeStats.critRate += 30;
                    }
                }
                else
                {
                    if (isPassiveActivated)
                    {
                        isPassiveActivated = false;
                        controller.runtimeStats.critRate -= 30;
                    }
                }
            }
        }
    }
}
