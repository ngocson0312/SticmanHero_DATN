using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class DualSword : Weapon
    {
        private int combo;
        private int passivePercent = 5;
        private int currentIndexMoveSet;
        private bool canDoCombo;
        public Transform leftWeapon;
        public Vector2 sizeCollider;
        public Vector2 offsetCollider;
        public ParticleSystem slashFX;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            Vector3 scale = leftWeapon.localScale;
            Quaternion rotation = leftWeapon.localRotation;
            Vector3 pos = leftWeapon.localPosition;
            leftWeapon.SetParent(controller.animatorHandle.GetBone(HumanBodyBones.LeftHand));
            leftWeapon.localScale = scale;
            leftWeapon.localRotation = rotation;
            leftWeapon.localPosition = pos;

            scale = slashFX.transform.localScale;
            rotation = slashFX.transform.localRotation;
            pos = slashFX.transform.localPosition;
            slashFX.transform.SetParent(controller.transform);

            slashFX.transform.localRotation = rotation;
            slashFX.transform.localPosition = pos;
            slashFX.transform.localScale = scale;
        }
        public override void SetActive(bool status)
        {
            base.SetActive(status);
            leftWeapon.gameObject.SetActive(status);
        }

        protected override void OnEvent(string obj)
        {
            if (!isTrigger) return;
            if (obj.Equals("TriggerDamage"))
            {
                var colls = new Collider2D[5];
                int hitDirection = controller.core.movement.facingDirection;
                Vector3 pos = controller.position + new Vector3(offsetCollider.x * hitDirection, offsetCollider.y);
                Physics2D.OverlapBoxNonAlloc(pos, sizeCollider, 0, colls, layerContact);
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.listEffect = listEffect;
                damageInfo.characterType = controller.characterType;
                damageInfo.hitDirection = hitDirection;

                bool isCrit = controller.IsCrit();
                int damage = (int)controller.runtimeStats.damage;

                damageInfo.stunTime = moveSets[currentIndexMoveSet].stunTime;
                damageInfo.impactSound = moveSets[currentIndexMoveSet].impactSound;
                damageInfo.idSender = controller.GetInstanceID();
                damageInfo.owner = controller;
                damageInfo.damageType = isCrit ? DamageType.CRITICAL : DamageType.NORMAL;

                damageInfo.onHitSuccess += (x) =>
                {
                    if (x == true)
                    {
                        controller.lifeStealing(damage);
                    }
                };
                combo++;
                switch (data.embedStone)
                {
                    case 1: // Sharp gem
                        damage += (int)controller.runtimeStats.damage * 20 / 100;
                        break;
                }
                int passiveDamage = 0;
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i] == null) continue;
                    IDamage target = colls[i].GetComponent<IDamage>();
                    if (target != null)
                    {
                        // Dual Sword Passive
                        if (combo == 3)
                        {
                            passiveDamage = target.controller.runtimeStats.health * passivePercent / 100;
                        }
                        else if (combo > 3)
                        {
                            combo = 1;
                            passiveDamage = 0;
                        }
                        var dmg = CharacterStats.GetDamageAfterReduction(damage, target.controller.runtimeStats.armor);
                        if (isCrit)
                        {
                            damageInfo.damage = dmg + (int)(dmg * controller.runtimeStats.critDamage / 100f) + passiveDamage;
                        }
                        else
                        {
                            damageInfo.damage = dmg + passiveDamage;
                        }
                        // if (target.controller != controller)
                        // {
                        //     isActiveCombo = true;
                        // }
                        target.TakeDamage(damageInfo);
                    }
                }
                canDoCombo = false;
            }
            if (obj.Equals("ActiveCombo"))
            {
                canDoCombo = true;
            }
            if (obj.Equals("DeactiveCombo"))
            {
                canDoCombo = false;
                isActiveCombo = false;
            }
            if (obj.Equals("EnableTrail"))
            {
                // EnableTrails(true);
            }
            if (obj.Equals("DisableTrail"))
            {
            }
            if (obj.Equals("PlayFX"))
            {
                AudioManager.Instance.PlayOneShot(moveSets[currentIndexMoveSet].activeSound, 1f);
                slashFX.Play();
            }
            if (obj.Equals("EmptyState"))
            {
                isActiveCombo = false;
            }
        }

        public override void OnEquip()
        {
            combo = 0;
            base.OnEquip();

        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
            leftWeapon.parent = transform;
            slashFX.transform.SetParent(transform);
        }

        public override void OnUpdate()
        {
            if (controller.isStunning)
            {
                isActiveCombo = false;
            }
        }
        public override void TriggerWeapon()
        {
            if (canDoCombo)
            {
                currentIndexMoveSet++;
                if (currentIndexMoveSet >= moveSets.Length)
                {
                    currentIndexMoveSet = 0;
                }
                isActiveCombo = PredictHit();
                var currentMoveSet = moveSets[currentIndexMoveSet];
                controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, 1, true, true);
                canDoCombo = false;
            }
            else
            {
                if (!controller.isInteracting && !canDoCombo)
                {
                    currentIndexMoveSet = 0;
                    var currentMoveSet = moveSets[currentIndexMoveSet];
                    isActiveCombo = PredictHit();
                    controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, 1, true, true);
                }
            }
        }
        private bool PredictHit()
        {
            var colls = new Collider2D[5];
            int hitDirection = controller.core.movement.facingDirection;
            Vector3 pos = controller.position + new Vector3(offsetCollider.x * hitDirection, offsetCollider.y);
            Physics2D.OverlapBoxNonAlloc(pos, sizeCollider, 0, colls, layerContact);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i] == null) continue;
                IDamage target = colls[i].GetComponent<IDamage>();
                if (target != null && target.controller != controller)
                {
                    return true;
                }
            }
            return false;
        }
        public override void ResetAnimation()
        {
            currentIndexMoveSet = 0;
            canDoCombo = false;
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
                case 2:
                    equipmentStats.damage = originalStats.damage * 10 / 100;
                    break;
                case 3:
                    equipmentStats.damage = originalStats.damage * 10 / 100;
                    equipmentStats.critRate = 10;
                    break;
                case 4:
                    equipmentStats.damage = originalStats.damage * 10 / 100;
                    equipmentStats.critRate = 10;
                    equipmentStats.critDamage = 20;
                    break;
                case 5:
                    equipmentStats.damage = originalStats.damage * 10 / 100;
                    equipmentStats.critRate = 10;
                    equipmentStats.critDamage = 20;
                    passivePercent = 8;
                    break;
                case 6:
                    equipmentStats.damage = originalStats.damage * 30 / 100;
                    equipmentStats.critRate = 10;
                    equipmentStats.critDamage = 20;
                    break;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (controller == null)
            {
                Vector3 pos = transform.position + (Vector3)offsetCollider;
                pos.z = 0;
                Gizmos.DrawWireCube(pos, sizeCollider);
            }
            else
            {
                Vector3 pos = controller.position + new Vector3(offsetCollider.x * controller.core.movement.facingDirection, offsetCollider.y);
                pos.z = 0;
                Gizmos.DrawWireCube(pos, sizeCollider);
            }
        }

        public override float GetDurability()
        {
            return 0;
        }
    }
}

