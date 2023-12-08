using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Enderman : GroundEnemy
    {
        public EndermanPatrolState endermanPatrolState;
        public EndermanChaseState endermanChaseState;
        public EndermanAttackState endermanAttackState;
        public override void Initialize()
        {
            base.Initialize();
            isActive = true;
            endermanPatrolState = new EndermanPatrolState(this, "");
            endermanChaseState = new EndermanChaseState(this, "");
            endermanAttackState = new EndermanAttackState(this, "");
            SwitchState(endermanPatrolState);
        }
        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effZombieDie);
        }
    }
}