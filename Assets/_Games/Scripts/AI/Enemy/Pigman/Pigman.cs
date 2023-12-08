using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SuperFight
{
    public class Pigman : GroundEnemy
    {
        private void Start()
        {
            Initialize();
        }

        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effZombieDie);
        }

    }
}