using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Necklace_Knight : Necklace
    {
        private int passiveData;
        private bool isPassiveUsed;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            isPassiveUsed = false;
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
            if (data.grade >= 5)
            {
                controller.AddDenyDeathCount();
            }
            base.OnEquip();
        }
        public override void OnUnEquip()
        {
            if (!isPassiveUsed && data.grade >= 5)
            {
                controller.RemoveDenyDeathCount();
            }
            base.OnUnEquip();
        }

        public override void OnUpdate()
        {
            if (data.grade >= 5) // Passive: neu HP duoi 1%, hoi 30% HP
            {
                passiveData = controller.originalStats.health * 30 / 100;
                if (controller.runtimeStats.health <= controller.originalStats.health * 0.1f && isPassiveUsed == false)
                {
                    controller.RemoveDenyDeathCount();
                    controller.AddHealth(passiveData);
                    isPassiveUsed = true;
                }
            }
        }
    }
}
