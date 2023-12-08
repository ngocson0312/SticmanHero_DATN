using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace SuperFight
{
    public class ExplosiveBow : Weapon
    {
        public Projectile arrow; // use fireball as prefab 
        public Transform posSpawn;
        public int numArrow = 50;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            // SetEquipmentStats(controller, grade, level);

        }
        public override void OnEquip()
        {
            base.OnEquip();
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
                    damageInfo.listEffect = listEffect;
                    damageInfo.hitDirection = hitDirection;

                    bool isCrit = controller.IsCrit();
                    int damage = (int)controller.runtimeStats.damage;

                    damageInfo.stunTime = moveSets[0].stunTime;
                    damageInfo.impactSound = moveSets[0].impactSound;
                    damageInfo.idSender = controller.core.combat.GetComponent<Collider2D>().GetInstanceID();
                    damageInfo.owner = controller;

                    damageInfo.onHitSuccess += (x) =>
                    {
                        if (x == true)
                        {
                            controller.lifeStealing(damage);
                        }
                    };

                    damageInfo.listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                    Vector2 direction = new Vector2(hitDirection, 0);


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
       
        public override void TriggerWeapon()
        {
            if (numArrow <= 0) return;
            if (!controller.isInteracting)
            {
                controller.animatorHandle.PlayAnimation("StraightBow", 0.1f, 1, true);
            }
        }

        public override void ResetAnimation()
        {

        }

        public override void OnUpdate()
        {

        }
        public override float GetDurability()
        {
            return 0;
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
            switch (data.embedStone)
            {
                case 1: // Sharp gem
                    // damage += (int)controller.runtimeStats.damage * 20 / 100;
                    break;
                case 2: // Fire gem
                    listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                    break;
                case 3: // Frost gem
                    listEffect.Add(new FrostbiteEffectData(controller, StatusEffectType.FROSTBITE));
                    break;
                case 4: // poison gem
                    listEffect.Add(new PoisonEffectData(controller, StatusEffectType.POISON));
                    break;
                case 5: // Blood gem
                    listEffect.Add(new BleedingEffectData(controller, StatusEffectType.BLEEDING));
                    break;
                case 6: // Poison gem
                    listEffect.Add(new ElectrocuteEffectData(controller, StatusEffectType.ELECTROCUTE));
                    break;
            }
            switch (data.grade)
            {
                case 2:
                    controller.runtimeStats.critRate += controller.runtimeStats.critRate * 10 / 100;
                    break;
                case 3:
                    controller.runtimeStats.critRate += controller.runtimeStats.critRate * 10 / 100;
                    controller.runtimeStats.critDamage += controller.runtimeStats.critDamage * 10 / 100;
                    break;
                case 4:
                    controller.runtimeStats.damage += controller.runtimeStats.damage * 10 / 100;
                    controller.runtimeStats.critRate += controller.runtimeStats.critRate * 10 / 100;
                    controller.runtimeStats.critDamage += controller.runtimeStats.critDamage * 10 / 100;
                    break;
                case 5:
                    controller.runtimeStats.damage += controller.runtimeStats.damage * 10 / 100;
                    controller.runtimeStats.critRate += controller.runtimeStats.critRate * 20 / 100;
                    controller.runtimeStats.critDamage += controller.runtimeStats.critDamage * 10 / 100;
                    break;
                case 6:
                    break;
            }
        }
    }
}
