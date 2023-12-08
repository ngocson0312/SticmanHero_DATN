using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Necklace_Rage : Necklace
    {
        private float timer;
        private int passiveDamage;
        private bool activeSkill;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);

        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
            equipmentStats = new CharacterStats();
            passiveDamage = originalStats.damage * 10 / 100;
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
            activeSkill = false;
            Controller.OnDead += TriggerSkill;
        }

        private void TriggerSkill(Controller _controller)
        {
            if (_controller == controller) return;
            if (!activeSkill)
            {
                controller.runtimeStats.damage += passiveDamage;
                activeSkill = true;
            }
            timer = 8f;
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
            Controller.OnDead -= TriggerSkill;
        }

        public override void OnUpdate()
        {
            if (data.grade >= 5) // Passive: giet quai tang 10% dmg
            {
                if (!activeSkill) return;
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    controller.runtimeStats.damage -= passiveDamage;
                    activeSkill = false;
                }
            }
        }
    }
}
