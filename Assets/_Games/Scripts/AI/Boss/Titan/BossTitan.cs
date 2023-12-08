using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class BossTitan : Boss
    {
        [Header("Charge Stats")]
        public LayerMask groundLayer;
        public LayerMask targetLayer;
        [Header("Particle")]
        public GameObject stunFX;
        public ParticleSystem FXBossTitanDir;
        public ParticleSystem FXBossTitanBlood;
        public GameObject GroundFake;
        public DamageCollider headDamage;
        public float speed = 10f;
        public TitanChargeState titanChargeState;
        public override void Initialize(BossFightArena bossFightArena)
        {
            stunFX.SetActive(false);
            base.Initialize(bossFightArena);
            core.movement.Flip();
            titanChargeState = new TitanChargeState(this, "");
            SwitchState(titanChargeState);
            headDamage.Initialize(this);
        }

        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            FXBossTitanBlood.Play();
            if (runtimeStats.health <= 0)
            {
                animatorHandle.PlayAnimation("Die", 0.1f, 1, false);
                if (GroundFake != null)
                {
                    GroundFake.SetActive(false);
                }
                core.movement.SetVelocityZero();
                Die(false);
            }
            else
            {
                animatorHandle.PlayAnimation("Hit", 0.1f, 1, true);
            }
        }

        public override void Active()
        {
            isActive = true;
        }
        protected override void UpdateLogic()
        {
            animatorHandle.SetBool("IsStunning", isStunning);
        }
        protected override void UpdatePhysic()
        {
        }
    }
}

