using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Creeper : GroundEnemy
    {
        public Renderer rendererCharacter;
        public CreeperExplodeState creeperExplodeState;
        public CreeperPatrolState creeperPatrol;
        public CreeperChaseState creeperChaseState;
        public ParticleSystem VfXExpolision;
        public AudioClip countDownSfx;
        public AudioClip explosionSfx;
        public override void Initialize()
        {
            base.Initialize();
            creeperPatrol = new CreeperPatrolState(this, "");
            creeperChaseState = new CreeperChaseState(this, "");
            creeperExplodeState = new CreeperExplodeState(this, "explode");

        }
        public override void ResetController()
        {
            base.ResetController();
            SwitchState(creeperPatrol);
            rendererCharacter.material.color = Color.white;
        }
        public override void Die(bool deactiveCharacter)
        {
            if (!isActive) return;
            base.Die(deactiveCharacter);
            healthBar.Deactive();
        }


    }
}

