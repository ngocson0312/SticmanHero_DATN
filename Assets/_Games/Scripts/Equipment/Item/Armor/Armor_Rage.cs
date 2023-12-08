using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Armor_Rage : Armor
    {
        private float timer = 5;
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
            if (data.grade >= 5)
            {
                controller.AddDenyDeathCount();
            }
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
            if (data.grade >= 5) // Passive: duoi 10% HP bat tu 5s
            {
                if (controller.runtimeStats.health <= controller.originalStats.health * 0.1f && isPassiveUsed == false)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        controller.RemoveDenyDeathCount();
                        controller.isInvincible = false;
                        isPassiveUsed = true;
                    }
                }
            }
        }
    }
}
