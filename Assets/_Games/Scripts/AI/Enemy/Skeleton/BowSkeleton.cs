using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace SuperFight
{
    public class BowSkeleton : Weapon
    {
        public Projectile arrow;
        public Transform posSpawn;

        public void SetEquipmentStats(Controller controller, int grade, int level)
        {

        }

        public override void OnUpdate()
        {

        }

        public override void ResetAnimation()
        {

        }

        public override void TriggerWeapon()
        {
            controller.animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
        }
        protected override void OnEvent(string eventName)
        {
            if (eventName.Equals("TriggerArrow"))
            {
                Projectile p = PoolManager.Pools["Projectile"].Spawn(arrow.transform).GetComponent<Projectile>();
                p.transform.position = posSpawn.position;
                int hitDirection = controller.core.movement.facingDirection;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.characterType = controller.characterType;
                damageInfo.hitDirection = hitDirection;

                bool isCrit = controller.IsCrit();
                int damage = (int)controller.runtimeStats.damage + (int)(controller.runtimeStats.damage * 0.8);

                damageInfo.stunTime = moveSets[0].stunTime;
                damageInfo.impactSound = moveSets[0].impactSound;
                damageInfo.idSender = controller.core.combat.GetComponent<Collider2D>().GetInstanceID();
                damageInfo.owner = controller;
                Vector2 direction = new Vector2(hitDirection, 0);

                p.OnContact += (idamages) =>
                {
                    for (int i = 0; i < idamages.Count; i++)
                    {
                        var dmg = damage;
                        if (isCrit)
                        {
                            damageInfo.damage = dmg + (int)(dmg * controller.runtimeStats.critDamage / 100f);
                        }
                        else
                        {
                            damageInfo.damage = dmg;
                        }
                        idamages[i].TakeDamage(damageInfo);
                    }
                };

                p.Initialize(direction, damageInfo);
            }
        }

        public override float GetDurability()
        {
            return 0;
        }
    }
}
