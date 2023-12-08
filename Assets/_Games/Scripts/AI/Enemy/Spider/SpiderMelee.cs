using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class SpiderMelee : Weapon
    {
        private int currentIndexMoveSet;
        private bool canDoCombo;
        public bool addPoison;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            if (addPoison)
            {
                listEffect.Add(new PoisonEffectData(controller, StatusEffectType.POISON));
            }
        }
        public override void SetUpPassive(CharacterStats originalStats)
        {
        }

        protected override void OnEvent(string obj)
        {
            if (obj.Equals("TriggerDamage"))
            {
                var colls = new Collider2D[3];
                Physics2D.OverlapCircleNonAlloc(controller.transform.position, attackRange, colls, layerContact);
                int hitDirection = controller.core.movement.facingDirection;
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
                            if (isCrit)
                            {
                                damageInfo.damage = damage + (int)(damage * controller.runtimeStats.critDamage / 100f);
                            }
                            else
                            {
                                damageInfo.damage = damage;
                            }

                            target.TakeDamage(damageInfo);
                        }
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
            }
        }
        public override void OnEquip()
        {

            base.OnEquip();
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
        public override float GetDurability()
        {
            return 0;
        }
    }
}
