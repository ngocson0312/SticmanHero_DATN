using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMecha : Boss
    {
        public LayerMask layerTarget;
        public Vector2 centerView;
        public Vector2 viewSize;
        public float moveAmount;
        public GreenMechaIdleState idleState;
        public GreenMechaChaseState chaseState;
        public GreenMechaAttackState attackState;
        public override void Initialize(BossFightArena arena)
        {
            base.Initialize(arena);
            // AudioManager.Instance.PlayAmbience("RainAmbient", 0.5f, true);
            idleState = new GreenMechaIdleState(this, "Idle");
            chaseState = new GreenMechaChaseState(this, "Chase");
            attackState = new GreenMechaAttackState(this, "Attack");
        }
        public override void Active()
        {
            isActive = true;
            SwitchState(idleState);
        }

        // public override void OnKillObject(int scoreReward)
        // {
        // }

        protected override void UpdateLogic()
        {
            isInteracting = animatorHandle.GetBool("IsInteracting");
            animatorHandle.SetFloat("MoveAmount", moveAmount);
        }

        protected override void UpdatePhysic()
        {
        }
      
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (runtimeStats.health <= 0)
            {
                Die(false);
                animatorHandle.PlayAnimation("Die", 0.1f, 1, false);
            }
        }
        public Controller GetTargetsInView()
        {
            Bounds bounds = new Bounds(transform.position + (Vector3)centerView, viewSize);
            if (bounds.Intersects(PlayerManager.GetBoundPlayer())) return PlayerManager.Instance.playerController;
            return null;
        }
        public void HandleLockTarget(Transform target, float snapValue)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            if (direction.x < 0 && core.movement.facingDirection == 1)
            {
                core.movement.Flip();
            }
            if (direction.x > 0 && core.movement.facingDirection == -1)
            {
                core.movement.Flip();
            }
            if (core.movement.facingDirection < 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 270, 0), snapValue);
            }
            if (core.movement.facingDirection > 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 100, 0), snapValue);
            }
        }

    }
}

