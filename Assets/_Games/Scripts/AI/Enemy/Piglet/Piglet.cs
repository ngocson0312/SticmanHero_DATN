using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Piglet : GroundEnemy
    {
        public PigletPatrolState patrolState;
        public PigletAttackState attackState;
        public AudioClip pigSfx;
        public AudioClip pigDieSfx;
        public override void Initialize()
        {
            base.Initialize();
            isActive = true;
            patrolState = new PigletPatrolState(this, "");
            attackState = new PigletAttackState(this, "");
        }
        public override void ResetController()
        {
            base.ResetController();
            SwitchState(patrolState);
        }
        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            AudioManager.Instance.PlayOneShot(pigDieSfx, 1f);
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effPigDie);
        }
    }
}