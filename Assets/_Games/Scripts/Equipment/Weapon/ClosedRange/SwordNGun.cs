using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class SwordNGun : Weapon
    {
        private int passiveDamage;
        private int passivePercent = 5;
        private int currentIndexMoveSet;
        private bool canDoCombo;
        public Transform gun;
        public Vector2 sizeCollider;
        public Vector2 offsetCollider;
        public ParticleSystem[] slashesFX;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            Vector3 scale = gun.localScale;
            Quaternion rotation = gun.localRotation;
            Vector3 pos = gun.localPosition;
            gun.SetParent(controller.animatorHandle.GetBone(HumanBodyBones.LeftHand));
            gun.localScale = scale;
            gun.localRotation = rotation;
            gun.localPosition = pos;

            for (int i = 0; i < slashesFX.Length; i++)
            {
                scale = slashesFX[i].transform.localScale;
                rotation = slashesFX[i].transform.localRotation;
                pos = slashesFX[i].transform.localPosition;
                slashesFX[i].transform.SetParent(controller.transform);

                slashesFX[i].transform.localRotation = rotation;
                slashesFX[i].transform.localPosition = pos;
                slashesFX[i].transform.localScale = scale;
            }
        }
        public override void SetActive(bool status)
        {
            base.SetActive(status);
            gun.gameObject.SetActive(status);
        }
        protected override void OnEvent(string obj)
        {
            if (!isTrigger) return;
            if (obj.Equals("TriggerGun"))
            {
                CheckDamage(true);
            }
            if (obj.Equals("TriggerDamage"))
            {
                CheckDamage(false);
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
            if (obj.Equals("EmptyState"))
            {
                isActiveCombo = false;
            }
            if (obj.Equals("PlayFX"))
            {
                AudioManager.Instance.PlayOneShot(moveSets[currentIndexMoveSet].activeSound, 1f);
                slashesFX[currentIndexMoveSet].Play();
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
        private void CheckDamage(bool isRange)
        {
            Collider2D[] colls = new Collider2D[5];
            int hitDirection = controller.core.movement.facingDirection;
            Vector3 pos = controller.position + new Vector3(offsetCollider.x * hitDirection, offsetCollider.y);
            Vector3 size = sizeCollider;
            DamageInfo damageInfo = new DamageInfo();
            if (isRange)
            {
                pos = controller.position + new Vector3(offsetCollider.x * 2 * controller.core.movement.facingDirection, offsetCollider.y);
                size.x *= 3;
                List<StatusEffectData> statusEffects = new List<StatusEffectData>(listEffect);
                statusEffects.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                damageInfo.listEffect = statusEffects;
            }
            else
            {
                damageInfo.listEffect = listEffect;
            }
            Physics2D.OverlapBoxNonAlloc(pos, size, 0, colls, layerContact);
            damageInfo.characterType = controller.characterType;
            damageInfo.hitDirection = hitDirection;

            bool isCrit = controller.IsCrit() || isRange;
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

            switch (data.embedStone)
            {
                case 1: // Sharp gem
                    damage += (int)controller.runtimeStats.damage * 20 / 100;
                    break;
            }
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i] == null) continue;
                Vector3 controllerPos = controller.transform.position;
                Vector3 targetPos = colls[i].transform.position;
                if (targetPos.y >= controllerPos.y - attackRange && (Vector2.Dot(Vector2.right * hitDirection, (targetPos - controllerPos).normalized) >= -0.2f || Mathf.Abs(targetPos.x - controllerPos.x) < .85f))
                {
                    IDamage target = colls[i].GetComponent<IDamage>();
                    if (target != null)
                    {
                        var dmg = CharacterStats.GetDamageAfterReduction(damage, target.controller.runtimeStats.armor);
                        if (isCrit)
                        {
                            damageInfo.damage = dmg + (int)(dmg * controller.runtimeStats.critDamage / 100f) + passiveDamage;
                        }
                        else
                        {
                            damageInfo.damage = dmg + passiveDamage;
                        }
                        target.TakeDamage(damageInfo);
                    }
                }
            }
        }
        public override void OnEquip()
        {
            base.OnEquip();
            if (controller.isStunning)
            {
                isActiveCombo = false;
            }
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
            gun.parent = transform;
            for (int i = 0; i < slashesFX.Length; i++)
            {
                slashesFX[i].transform.SetParent(transform);
            }
        }

        public override void OnUpdate()
        {
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
                controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, "Override", true, true);
                canDoCombo = false;
            }
            else
            {
                if (!controller.isInteracting && !canDoCombo)
                {
                    isActiveCombo = PredictHit();
                    currentIndexMoveSet = 0;
                    var currentMoveSet = moveSets[currentIndexMoveSet];
                    controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, "Override", true, true);
                }
            }
        }
        public override float GetDurability()
        {
            return 0;
        }
        public override void ResetAnimation()
        {
            currentIndexMoveSet = 0;
            canDoCombo = false;
            isActiveCombo = false;
        }

        public override void SetUpPassive(CharacterStats originalStats)
        {
            equipmentStats = new CharacterStats();
            switch (data.embedStone)
            {
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
                    equipmentStats.critDamage = 70;
                    break;
                case 6:
                    equipmentStats.damage = originalStats.damage * 30 / 100;
                    equipmentStats.critRate = 10;
                    equipmentStats.critDamage = 70;
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
                Gizmos.color = Color.yellow;
                pos.x *= 3;
                Gizmos.DrawWireCube(pos, new Vector3(sizeCollider.x * 3, sizeCollider.y));
            }
            else
            {
                Vector3 pos = controller.position + new Vector3(offsetCollider.x * controller.core.movement.facingDirection, offsetCollider.y);
                pos.z = 0;
                Gizmos.DrawWireCube(pos, sizeCollider);
                Gizmos.color = Color.yellow;
                pos = controller.position + new Vector3(offsetCollider.x * 3 * controller.core.movement.facingDirection, offsetCollider.y);
                Gizmos.DrawWireCube(pos, new Vector3(sizeCollider.x * 3, sizeCollider.y));
            }
        }
    }
}

