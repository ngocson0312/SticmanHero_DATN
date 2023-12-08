using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigManSword : GroundEnemy
    {
        public PigmanSwordPatrol pigmanSwordPatrol;
        public PigmanSwordChase pigmanSwordChaseSate;
        public PigmanSwordAttackState pigmanSwordAttack;
        public float moveAmount;
        public override void Initialize()
        {
            base.Initialize();
            pigmanSwordPatrol = new PigmanSwordPatrol(this, "");
            pigmanSwordChaseSate = new PigmanSwordChase(this, "");
            pigmanSwordAttack = new PigmanSwordAttackState(this, "");
            isActive = true;
            SwitchState(pigmanSwordPatrol);
        }

        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effZombieDie);
        }

        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            animatorHandle.SetFloat("MoveAmount", moveAmount);
        }
    }
}
