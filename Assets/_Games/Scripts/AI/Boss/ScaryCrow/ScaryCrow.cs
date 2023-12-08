using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ScaryCrow : Boss
    {
        public ScaryCrowAnimator scaryCrowAnimator { get; set; }
        public LayerMask layerTarget;
        public Vector2 centerView;
        public Vector2 viewSize;
        public ScaryCrowIdleState idleState;
        public ScaryCrowChaseState chaseState;
        public ScaryCrowAttackState attackState;
        public int moveAmount;
        public bool rotateWhenAction;
        public override void Initialize(BossFightArena arena)
        {
            base.Initialize(arena);
            idleState = new ScaryCrowIdleState(this, "Idle");
            chaseState = new ScaryCrowChaseState(this, "Chase");
            attackState = new ScaryCrowAttackState(this, "Attack");
            scaryCrowAnimator = (ScaryCrowAnimator)animatorHandle;
        }
        public override void Active()
        {
            SwitchState(idleState);
            isActive = true;
        }



        protected override void UpdateLogic()
        {
            animatorHandle.SetFloat("MoveAmount", moveAmount);
            isInteracting = animatorHandle.GetBool("IsInteracting");
            HandleRotate();
        }

        protected override void UpdatePhysic()
        {
        }
        void HandleRotate()
        {
            if (isInteracting) return;
            if (core.movement.facingDirection < 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 270, 0), 0.5f);
            }
            if (core.movement.facingDirection > 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 100, 0), 0.5f);
            }
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
        public Collider2D[] GetTargetsInView()
        {
            Collider2D[] colls = Physics2D.OverlapBoxAll(transform.position + (Vector3)centerView, viewSize, 0, layerTarget);
            List<Collider2D> listColl = new List<Collider2D>();
            for (int i = 0; i < colls.Length; i++)
            {
                if (!colls[i].Equals(core.combat.GetComponent<Collider2D>()) && colls[i].GetComponent<Combat>().getType != characterType)
                {
                    listColl.Add(colls[i]);
                }
            }
            return listColl.ToArray();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)centerView, viewSize);
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}

