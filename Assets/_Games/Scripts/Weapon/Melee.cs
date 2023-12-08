using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Melee : Weapon
    {
        private int currentIndexMoveSet;
        public Vector2 sizeCollider;
        public Vector2 offsetCollider;
        private bool canDoCombo;

        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
        }
        protected override void OnEvent(string obj)
        {
            if (obj.Equals("TriggerDamage"))
            {
                var colls = new Collider2D[5];
                int hitDirection = controller.core.movement.facingDirection;
                Vector3 pos = controller.position + new Vector3(offsetCollider.x * hitDirection, offsetCollider.y);
                Physics2D.OverlapBoxNonAlloc(pos, sizeCollider, 0, colls, layerContact);
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.listEffect = new List<StatusEffectData>();
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
                    }
                };

                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i] == null) continue;
                    IDamage target = colls[i].GetComponent<IDamage>();
                    if (target != null)
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
            }
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
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
                var currentMoveSet = moveSets[currentIndexMoveSet];
                AudioManager.Instance.PlayOneShot(currentMoveSet.activeSound, 1f);
                controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, 1, true);
                canDoCombo = false;
            }
            else
            {
                if (!controller.isInteracting && !canDoCombo)
                {
                    currentIndexMoveSet = 0;
                    var currentMoveSet = moveSets[currentIndexMoveSet];
                    AudioManager.Instance.PlayOneShot(currentMoveSet.activeSound, 1f);
                    controller.animatorHandle.PlayAnimation(currentMoveSet.animationName, 0.1f, 1, true);
                }
            }
        }


        public override void ResetAnimation()
        {
            currentIndexMoveSet = 0;
            canDoCombo = false;
        }

        public void SetEquipmentStats(Controller controller, int grade, int level)
        {

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
