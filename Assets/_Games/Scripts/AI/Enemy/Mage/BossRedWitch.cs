using SuperFight;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BossRedWitch : GroundEnemy
{
    public MegaPatrolState patrol;
    public MageAttackStates attackState;
    public float radius = 10f;
    public LayerMask layerMask;
    public override void Initialize()
    {
        base.Initialize();
        patrol = new MegaPatrolState(this, "patrol");
        attackState = new MageAttackStates(this, "attack");
        animatorHandle.OnEventAnimation += OnEvent;
    }

    private void OnEvent(string obj)
    {
        if (obj.Equals("Buff"))
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
            for (int i = 0; i < colls.Length; i++)
            {
                IDamage id = colls[i].GetComponent<IDamage>();
                if (id != null && id.controller != this && id.controller.characterType != CharacterType.Character)
                {
                    int health = id.controller.runtimeStats.health + (int)(id.controller.originalStats.health * 0.3f);

                    id.controller.SetHealth(health);
                }
            }
        }
    }

    public override void ResetController()
    {
        base.ResetController();
        SwitchState(attackState);
    }
    // public override void Die(bool deactiveCharacter)
    // {
    //     base.Die(deactiveCharacter);
    //    // SoundManager.Instance.playSoundFx(SoundManager.Instance.effZombieDie);
    // }

    // public override void DetectPlayer()
    // {
    //     base.DetectPlayer();
    //     if (delayTimePlaySoundFx <= 0)
    //     {
    //         delayTimePlaySoundFx = 5f;
    //        // SoundManager.Instance.playRandFx(new AudioClip[] { SoundManager.Instance.effZombie1, SoundManager.Instance.effZombie2, SoundManager.Instance.effZombie3 });
    //     }
    // }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
