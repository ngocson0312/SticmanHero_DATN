using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;
namespace SuperFight
{
    public class Bow : Weapon
    {
        public Projectile arrow;
        public Transform posSpawn;
        public int numArrow;
        const int MaxArrow = 10;
        const float ReloadTime = 5;
        private Animator animator;
        public AudioClip shootSfx;
        private float reloadTimer;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            animator = GetComponentInChildren<Animator>();
            Vector3 scale = transform.localScale;
            Quaternion rotation = transform.localRotation;
            Vector3 pos = transform.localPosition;
            transform.SetParent(controller.animatorHandle.GetBone(HumanBodyBones.LeftHand));
            transform.localScale = scale;
            transform.localRotation = rotation;
            transform.localPosition = pos;
            numArrow = MaxArrow;
            reloadTimer = 0;
        }

        public override void OnEquip()
        {
            base.OnEquip();
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
        }
        public override float GetDurability()
        {
            return (float)numArrow / (float)MaxArrow;
        }
        public override float GetReloadNormalizedTime()
        {
            return reloadTimer / ReloadTime;
        }
        protected override void OnEvent(string obj)
        {
            if (!isTrigger) return;
            if (obj.Equals("EmptyState"))
            {
                isActiveCombo = false;
            }
            if (obj.Equals("TriggerDamage"))
            {
                AudioManager.Instance.PlayOneShot(shootSfx, 1f);
                Projectile p = PoolManager.Pools["Projectile"].Spawn(arrow.transform).GetComponent<Projectile>();
                p.transform.position = posSpawn.position;
                int hitDirection = controller.core.movement.facingDirection;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.characterType = controller.characterType;
                damageInfo.hitDirection = hitDirection;
                //Bow Passive
                // int distanceMultiplier = (int)Mathf.Abs(p.hitTransform.x - posSpawn.position.x);
                // if (distanceMultiplier > attackRange)
                // {
                //     distanceMultiplier = (int)attackRange;
                // }
                bool isCrit = controller.IsCrit();
                int damage = (int)controller.runtimeStats.damage;

                damageInfo.stunTime = moveSets[0].stunTime;
                damageInfo.impactSound = moveSets[0].impactSound;
                damageInfo.idSender = controller.core.combat.getColliderInstanceID;
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
                        var newDamage = CharacterStats.GetDamageAfterReduction(damage, idamages[i].controller.runtimeStats.armor);
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
                if (numArrow <= 0)
                {
                    numArrow = 0;
                    reloadTimer = ReloadTime;
                }
                p.Initialize(direction, damageInfo);
                isActiveCombo = false;
            }
        }

        public override void TriggerWeapon()
        {
            if (numArrow <= 0) return;
            if (!controller.isInteracting)
            {
                isActiveCombo = true;
                animator.CrossFade("Shot", 0.1f);
                controller.animatorHandle.PlayAnimation("StraightBow", 0.1f, 1, true, true);
            }
        }

        public override void ResetAnimation()
        {

        }

        public override void OnUpdate()
        {
            if (numArrow > 0) return;
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                numArrow = MaxArrow;
            }
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
            equipmentStats = new CharacterStats();
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
                case 3:
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    break;
                case 4:
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
                case 5:
                    equipmentStats.damage += originalStats.damage * 10 / 100;
                    equipmentStats.critRate += originalStats.critRate * 10 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
                case 6:
                    equipmentStats.damage += originalStats.damage * 10 / 100;
                    equipmentStats.critRate += originalStats.critRate * 20 / 100;
                    equipmentStats.critDamage += originalStats.critDamage * 10 / 100;
                    break;
            }
        }
    }

}
