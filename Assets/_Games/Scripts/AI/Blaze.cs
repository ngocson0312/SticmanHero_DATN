using SuperFight;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaze : GroundEnemy
{

    public BlazePatrolState BlazePatrolState;
    public BlazeChaseState BlazeChaseState;
    public BlazeAttackState BlazeAttackState;
    public FireBall fireBall;
    public Transform PosFire;
    public override void Initialize()
    {
        base.Initialize();
        isActive = true;
        BlazePatrolState = new BlazePatrolState(this, "");
        BlazeChaseState = new BlazeChaseState(this, "");
        BlazeAttackState = new BlazeAttackState(this, "");
    }
    public override void ResetController()
    {
        base.ResetController();
        SwitchState(BlazePatrolState);
        core.movement.SetGravityScale(0);
    }
    public override void Die(bool deactiveCharacter)
    {
        base.Die(deactiveCharacter);
        core.movement.SetGravityScale(1);
    }
    public override void OnTakeDamage(DamageInfo damageInfo)
    {
        base.OnTakeDamage(damageInfo);
        if (NormalizeHealth() > 0 && damageInfo.stunTime > 0 && !isUnstopable)
        {
            animatorHandle.PlayAnimation("Stun", 0.1f, 1, true);
        }
    }
}
