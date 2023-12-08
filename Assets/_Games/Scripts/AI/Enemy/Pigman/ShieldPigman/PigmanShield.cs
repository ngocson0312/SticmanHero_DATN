using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SuperFight
{
    public class PigmanShield : GroundEnemy
    {
        public PigmanShieldPatrol pigmanShieldPatrol;
        public PigmanChaseSate pigmanShieldChase;
        public PigmanShieldAttackState pigmanShieldAttackState;
        public override void Initialize()
        {
            base.Initialize();
            pigmanShieldPatrol = new PigmanShieldPatrol(this, "");
            pigmanShieldChase = new PigmanChaseSate(this, "");
            pigmanShieldAttackState = new PigmanShieldAttackState(this, "");
            isActive = true;
            SwitchState(pigmanShieldPatrol);
        }

        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (damageInfo.hitDirection != 0 && damageInfo.hitDirection != core.movement.facingDirection) return;
            base.OnTakeDamage(damageInfo);
        }

        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            animatorHandle.SetFloat("MoveAmount", core.movement.currentVelocity.x != 0 ? 1 : 0);
        }

    }
}