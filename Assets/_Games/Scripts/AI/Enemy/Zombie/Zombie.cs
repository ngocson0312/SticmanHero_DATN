using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Zombie : GroundEnemy
    {
        public ZombiePatrolState zombiePatrolState;
        public ZombieChaseState zombieChaseState;
        public ZombieAttackState zombieAttackState;
        public ZombieJumpAttackState zombieJumpAttackState;
        public float moveAmount;
        public AudioClip zombieSound;
        public AudioClip zombieDeathSound;
        public override void Initialize()
        {
            base.Initialize();
            zombiePatrolState = new ZombiePatrolState(this, "");
            zombieChaseState = new ZombieChaseState(this, "");
            zombieAttackState = new ZombieAttackState(this, "");
            zombieJumpAttackState = new ZombieJumpAttackState(this, "");
        }
        public override void ResetController()
        {
            base.ResetController();
            SwitchState(zombiePatrolState);
        }
        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            AudioManager.Instance.PlayOneShot(zombieDeathSound, 1f);
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effZombieDie);
        }
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            isInteracting = animatorHandle.GetBool("IsInteracting");
            animatorHandle.SetFloat("MoveAmount", moveAmount);
        }
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (NormalizeHealth() > 0 && damageInfo.stunTime > 0)
            {
                animatorHandle.PlayAnimation("Stun", 0.1f, 1, true);
            }
        }
    }
}