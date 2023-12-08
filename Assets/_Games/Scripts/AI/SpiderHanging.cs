using SuperFight;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderHanging : Enemy
{
    public float moveTime;
    public LineRenderer silk;
    public Transform firePoint;
    public Arrow arrowPrefab;
    public float moveRange;
    public float firstDelay;
    public MoveState moveState;
    public override void Initialize()
    {
        base.Initialize();
        moveState = new MoveState(this, "attack");
    }
    public override void ResetController()
    {
        base.ResetController();
        SwitchState(moveState);
        core.movement.SetBodyType(RigidbodyType2D.Kinematic);
        core.movement.SetGravityScale(0f);
    }
    public override void OnTakeDamage(DamageInfo damageInfo)
    {
        base.OnTakeDamage(damageInfo);
        healthBar.UpdateBar(NormalizeHealth());
        if (NormalizeHealth() <= 0)
        {
            StopStateMachine();
            animatorHandle.PlayAnimation("Dead", 0.1f, 1, true);
            core.movement.SetBodyType(RigidbodyType2D.Dynamic);
            core.movement.SetGravityScale(1f);
            core.movement.SetVelocity(new Vector2(damageInfo.hitDirection, 1), 10);
            Die(false);
        }
    }
    public void OnTouchPlayer(IDamage idamage)
    {
        DamageInfo damageInfo = new DamageInfo();
        damageInfo.damage = runtimeStats.damage;
        damageInfo.owner = this;
        damageInfo.characterType = characterType;
        damageInfo.stunTime = 0.1f;
        idamage.TakeDamage(damageInfo);
    }
    public override Controller GetTargetInView()
    {
        return null;
    }

    public override Controller GetTargetInView(Bounds bounds)
    {
        throw new System.NotImplementedException();
    }
}
