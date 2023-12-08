using SuperFight;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : GroundEnemy
{
    [SerializeField] GameObject warning;
    [SerializeField] Projectile projectile;
    public GolemPatrol golemPatrol;
    public GolemChase golemChase;
    public GolemAttack golemAttack;
    public float moveAmount;
    public override void Initialize()
    {
        base.Initialize();
        isActive = true;
        golemPatrol = new GolemPatrol(this, "");
        golemChase = new GolemChase(this, "");
        golemAttack = new GolemAttack(this, "");
        SwitchState(golemPatrol);
        animatorHandle.OnEventAnimation += OnEvent;
        warning.SetActive(false);
    }
    protected override void LogicUpdate()
    {
        base.LogicUpdate();
        animatorHandle.SetFloat("MoveAmount", moveAmount);
    }
    private void OnEvent(string obj)
    {
        if (obj.Equals("EnableWarning"))
        {
            ActiveWarning();
        }
        if (obj.Equals("TriggerDamage"))
        {
            Attack();
        }
    }

    public override void Die(bool deactiveCharacter)
    {
        base.Die(deactiveCharacter);
        warning.SetActive(false);
    }
    public void ActiveWarning()
    {
        warning.SetActive(true);
    }

    public void Attack()
    {
        warning.SetActive(false);
        CameraController.Instance.ShakeCamera(.35f, 0.7f, 10, 90, true);
        DamageInfo damageInfo = new DamageInfo();
        damageInfo.damage = runtimeStats.damage;
        damageInfo.owner = this;
        damageInfo.characterType = characterType;
        damageInfo.idSender = core.combat.getColliderInstanceID;
        Projectile p = FactoryObject.Spawn<Projectile>("Projectile", projectile.transform);
        p.OnContact += (x) =>
        {
            for (int i = 0; i < x.Count; i++)
            {
                x[i].TakeDamage(damageInfo);
            }
        };
        p.transform.position = transform.position + Vector3.up * 0.5f;
        p.Initialize(Vector2.right, damageInfo);
        p = FactoryObject.Spawn<Projectile>("Projectile", projectile.transform);
        p.OnContact += (x) =>
        {
            for (int i = 0; i < x.Count; i++)
            {
                x[i].TakeDamage(damageInfo);
            }
        };
        p.transform.position = transform.position + Vector3.up * 0.5f;
        p.Initialize(Vector2.left, damageInfo);
    }
}
