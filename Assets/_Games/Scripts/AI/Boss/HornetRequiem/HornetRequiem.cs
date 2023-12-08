using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HornetRequiem : Boss
    {
        public LayerMask layerTarget;
        public Vector2 centerView;
        public Vector2 viewSize;
        public HornetRequiemIdleState idleState;
        public HornetRequiemChaseState chaseState;
        public HornetRequiemAttackState attackState;
        public ParticleSystem trail;
        public bool rotateWhenAction;
        public float moveAmount;
        public float xMax;
        public float xMin;
        public override void Initialize(BossFightArena arena)
        {
            base.Initialize(arena);
            idleState = new HornetRequiemIdleState(this, "Idle");
            chaseState = new HornetRequiemChaseState(this, "Chase");
            attackState = new HornetRequiemAttackState(this, "Attack");
            animatorHandle.ResetAnimator();
        }
        public override void Active()
        {
            isActive = true;
            SwitchState(idleState);
        }

        protected override void UpdateLogic()
        {
            isInteracting = animatorHandle.GetBool("IsInteracting");
            animatorHandle.SetFloat("MoveAmount", moveAmount);
            trail.transform.position = transform.position + Vector3.up * 2;
        }

        protected override void UpdatePhysic()
        {
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
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (runtimeStats.health <= 0)
            {
                animatorHandle.PlayAnimation("Die", 0.1f, 1, false);
                Die(false);
            }
        }
        public void DisableGravity()
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        public void EnableGravity()
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }
}

