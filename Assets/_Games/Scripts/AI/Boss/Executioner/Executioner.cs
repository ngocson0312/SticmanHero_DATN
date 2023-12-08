using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Executioner : Boss
    {
        public float speed = 4f;
        public LayerMask layerTarget;
        public Vector2 centerView;
        public Vector2 viewSize;
        public float moveAmount;
        public float xMax;
        public float xMin;
        public ExecutionerIdleState idleState;
        public ExecutionerAttackState attackState;
        public bool rotateWhenAction;
        public override void Initialize(BossFightArena arena)
        {
            base.Initialize(arena);
            idleState = new ExecutionerIdleState(this, "Idle");
            attackState = new ExecutionerAttackState(this, "Attack");
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
        }
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (runtimeStats.health <= 0)
            {
                Die(false);
                animatorHandle.PlayAnimation("Die", 0.1f, 1, true);
                isActive = false;
            }
            else
            {
                animatorHandle.PlayAnimation("Hit", 0.1f, 0, false);
            }
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
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 90, 0), snapValue);
            }
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
            return PlayerManager.Instance.playerController;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)centerView, viewSize);
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }

}
