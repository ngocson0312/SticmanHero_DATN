using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Renek : Boss
    {
        public RenekAnimator renekAnimator { get; set; }
        public LayerMask layerTarget;
        public Vector2 centerView;
        public Vector2 viewSize;
        public float moveAmount;
        public RenekIdleState idleState;
        public RenekAttackState attackState;
        public RenekChaseState chaseState;
        public RenekPursuitState pursuitState;
        public override void Initialize(BossFightArena arena)
        {
            base.Initialize(arena);
            idleState = new RenekIdleState(this, "Idle");
            chaseState = new RenekChaseState(this, "Chase");
            attackState = new RenekAttackState(this, "Attack");
            pursuitState = new RenekPursuitState(this, "Pursuit");
            renekAnimator = (RenekAnimator)animatorHandle;
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
            core.combat.transform.localPosition = new Vector3(core.movement.facingDirection, 0, 0);
        }

        protected override void UpdatePhysic()
        {
        }
      
       
        public Controller GetTargetsInView()
        {
            // Collider2D[] colls = Physics2D.OverlapBoxAll(transform.position + (Vector3)centerView, viewSize, 0, layerTarget);
            // List<Collider2D> listColl = new List<Collider2D>();
            // for (int i = 0; i < colls.Length; i++)
            // {
            //     if (!colls[i].Equals(core.combat.GetComponent<Collider2D>()) && colls[i].GetComponent<Combat>().getType != characterType)
            //     {
            //         listColl.Add(colls[i]);
            //     }
            // }
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
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (runtimeStats.health <= 0)
            {
                Die(false);
                animatorHandle.PlayAnimation("Die", 0.1f, 1, true);
            }
            else
            {
                animatorHandle.PlayAnimation("Hit", 0.1f, 0, false);
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)centerView, viewSize);
        }
    }

}

