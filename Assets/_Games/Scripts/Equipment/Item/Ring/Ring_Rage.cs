using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Ring_Rage : Ring
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
                    equipmentStats.health += 50;
                    break;
                case 4:
                    equipmentStats.health += 50;
                    equipmentStats.health += originalStats.health * 5 / 100;
                    break;
                case 5:
                    equipmentStats.health += 50;
                    equipmentStats.health += originalStats.health * 5 / 100;
                    break;
                case 6:
                    equipmentStats.health += 50;
                    equipmentStats.health += originalStats.health * 20 / 100;
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
            if (data.grade >= 5) // Passive: duoi 50% HP tang 10% crit rate
            {
                if (controller.runtimeStats.health <= controller.originalStats.health / 2)
                {
                    if (!isPassiveActivated)
                    {
                        isPassiveActivated = true;
                        controller.runtimeStats.critRate += 10;
                    }
                }
                else
                {
                    if (isPassiveActivated)
                    {
                        isPassiveActivated = false;
                        controller.runtimeStats.critRate -= 10;
                    }
                }
            }
        }
    }
}
