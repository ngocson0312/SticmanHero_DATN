using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class GreatSword : Weapon
    {
        private int currentIndexMoveSet;
        private bool canDoCombo;
        public Vector2 sizeCollider;
        public Vector2 offsetCollider;
        public ParticleSystem[] slashesFX;
        public AudioClip thirdStrikeSound;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            for (int i = 0; i < slashesFX.Length; i++)
            {
                Vector3 scale = slashesFX[i].transform.localScale;
                Quaternion rotation = slashesFX[i].transform.localRotation;
                Vector3 pos = slashesFX[i].transform.localPosition;
                slashesFX[i].transform.SetParent(controller.transform);

                slashesFX[i].transform.localRotation = rotation;
                slashesFX[i].transform.localPosition = pos;
                slashesFX[i].transform.localScale = scale;
            }

        }
        protected override void OnEvent(string obj)
        {
            if (!isTrigger) return;
            if (obj.Equals("PlayFX"))
            {
                AudioManager.Instance.PlayOneShot(moveSets[currentIndexMoveSet].activeSound, 1f);
                slashesFX[currentIndexMoveSet].Play();
            }
            if (obj.Equals("TriggerDamage"))
            {
                if (currentIndexMoveSet == 2)
                {
                    AudioManager.Instance.PlayOneShot(thirdStrikeSound, 1f);
                }
                var colls = new Collider2D[5];
                int hitDirection = controller.core.movement.facingDirection;
                Vector3 pos = controller.position + new Vector3(offsetCollider.x * hitDirection, offsetCollider.y);
                Physics2D.OverlapBoxNonAlloc(pos, sizeCollider, 0, colls, layerContact);
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.listEffect = listEffect;
                damageInfo.characterType = controller.characterType;
                damageInfo.hitDirection = hitDirection;
                damageInfo.stunForce = moveSets[currentIndexMoveSet].force;
                bool isCrit = controller.IsCrit();
                int damage = (int)controller.runtimeStats.damage;
                damage += (int)(damage * 0.5f);

                damageInfo.stunTime = moveSets[currentIndexMoveSet].stunTime;
                damageInfo.impactSound = moveSets[currentIndexMoveSet].impactSound;
                damageInfo.idSender = controller.GetInstanceID();
                damageInfo.owner = controller;
                damageInfo.damageType = isCrit ? DamageType.CRITICAL : DamageType.NORMAL;

                // Sword Passive
                damageInfo.onHitSuccess += (x) =>
                {
                    if (x == true)
                    {
                        controller.lifeStealing(damage);
                    }

                };

                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i] == null) continue;
                    IDamage target = colls[i].GetComponent<IDamage>();
                    if (target != null)
                    {
                        var dmg = CharacterStats.GetDamageAfterReduction(damage, target.controller.runtimeStats.armor);
                        if (isCrit)
                        {
                            int critDmgBonus = 100;
                            if (data.grade >= 5)
                            {
                                critDmgBonus = 150;
                            }
                            damageInfo.damage = dmg + (int)(dmg * (controller.runtimeStats.critDamage + critDmgBonus) / 100f);
                        }
                        else
                        {
                            damageInfo.damage = dmg;
                        }
                        target.TakeDamage(damageInfo);
                    }
                }
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
        }
        public override void OnEquip()
        {
            base.OnEquip();
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
            for (int i = 0; i < slashesFX.Length; i++)
            {
                slashesFX[i].transform.SetParent(transform);
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
                var currentMoveSet = moveSets[currentIndexMoveSet];
                isActiveCombo = PredictHit();
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
                    break;
                case 6:
                    equipmentStats.damage = originalStats.damage * 30 / 100;
                    equipmentStats.critRate = 10;
                    equipmentStats.critDamage = 20;
                    break;
            }
        }
        public override float GetDurability()
        {
            return 0;
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
    }
}
