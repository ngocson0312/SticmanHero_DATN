using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class WitherSkeltonBomPack : Weapon
    {
        public Bomb bombPrefab;


        public override void OnUpdate()
        {

        }
        protected override void OnEvent(string obj)
        {
            Bomb bomb = Instantiate(bombPrefab);
            bomb.transform.position = transform.position;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.owner = controller;
            damageInfo.listEffect = new List<StatusEffectData>();
            damageInfo.listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
            bomb.ActiveBomb(PlayerManager.Instance.transform.position, damageInfo);
        }
        public override void OnEquip()
        {
            base.OnEquip();
        }

        public override void OnUnEquip()
        {
            base.OnUnEquip();
        }


        public override void TriggerWeapon()
        {

            controller.animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);

        }
        public override void ResetAnimation()
        {

        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public override void SetUpPassive(CharacterStats originalStats)
        {

        }
        public override float GetDurability()
        {
            return 0;
        }
    }
}
