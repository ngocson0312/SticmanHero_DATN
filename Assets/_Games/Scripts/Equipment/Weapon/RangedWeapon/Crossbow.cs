using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace SuperFight
{
    public class Crossbow : Weapon
    {
        public Projectile arrow;
        public Transform posSpawn;
        public int numArrow = 100;
        private float fireRateTimer;
        private bool isActive;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            // SetEquipmentStats(controller, grade, level);

        }

        public override void OnEquip()
        {
            base.OnEquip();
            isActive = false;
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
        }

        protected override void OnEvent(string obj)
        {
            // if (currentWeapon != null && currentWeapon != this) return;
            if (obj.Equals("SendDamage"))
            {
                isActive = true;
            }
        }
     
        public override void TriggerWeapon()
        {
            if (numArrow <= 0) return;
            if (!controller.isInteracting)
            {
                controller.animatorHandle.PlayAnimation("StraightBow", 0.1f, 1, false);
            }
            fireRateTimer -= Time.deltaTime;
            if (fireRateTimer <= 0 && isActive == true)
            {
                fireRateTimer = 0.15f;
                if (numArrow <= 0)
                {
                    Debug.Log("Out of arrow");
                }
                else
                {
                    Projectile p = PoolManager.Pools["Projectile"].Spawn(arrow.transform).GetComponent<Projectile>();
                    p.transform.position = posSpawn.position;
                    int hitDirection = controller.core.movement.facingDirection;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.characterType = controller.characterType;
                    damageInfo.hitDirection = hitDirection;

                    bool isCrit = controller.IsCrit();
                    int damage = (int)controller.runtimeStats.damage;

                    damageInfo.stunTime = moveSets[0].stunTime;
                    damageInfo.impactSound = moveSets[0].impactSound;
                    damageInfo.idSender = controller.core.combat.GetComponent<Collider2D>().GetInstanceID();
                    damageInfo.owner = controller;
                    Vector2 direction = new Vector2(hitDirection, 0);

                    damageInfo.onHitSuccess += (x) =>
                    {
                        if (x == true)
                        {
                            controller.lifeStealing(damage);
                        }
                    };


                    switch (data.embedStone)
                    {
                        case 1: // Sharp gem
                            damage += (int)controller.runtimeStats.damage * 20 / 100;
                            break;
                    }

                    p.OnContact += (idamages) =>
                     {
                         for (int i = 0; i < idamages.Count; i++)
                         {
                             var newDamage = damage - idamages[i].controller.runtimeStats.armor;
                             if (isCrit)
                             {
                                 damageInfo.damage = newDamage + (int)(newDamage * controller.runtimeStats.critDamage / 100f);
                             }
                             else
                             {
                                 damageInfo.damage = newDamage;
                             }
                             idamages[i].TakeDamage(damageInfo);
                         }
                     };


                    numArrow--;
                    p.Initialize(direction, damageInfo);
                }
            }



        }

        public override void ResetAnimation()
        {

        }

        public override void OnUpdate()
        {

        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
            equipmentStats = new CharacterStats();
            switch (data.grade)
            {
                case 1:
                    break;
                case 2:
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    break;
                case 3:
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
                case 4:
                    equipmentStats.damage += originalStats.damage * 10 / 100;
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
                case 5:
                    equipmentStats.damage += originalStats.damage * 10 / 100;
                    equipmentStats.critRate += originalStats.critRate * 20 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
            }
        }

        public override float GetDurability()
        {
            return 0;
        }
    }
}
