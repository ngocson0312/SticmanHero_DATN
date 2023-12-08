using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GroundEnemy : Enemy
    {
        public Vector2 offsetView;
        public Vector2 viewBounds;
        public float attackSpeed = 2;
        public float attackRange { get { return weapon.attackRange; } }
        public override void Initialize()
        {
            base.Initialize();
            healthBar.UpdateBar(1);
            core.Active();
        }
        public override Controller GetTargetInView()
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(offsetView.x * core.movement.facingDirection, offsetView.y);
            Bounds bounds = new Bounds(origin, viewBounds);
            Bounds playerBound = PlayerManager.GetBoundPlayer();
            Vector2 direction = playerBound.center - transform.position;

            if (bounds.Intersects(playerBound))
            {
                if (!Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, LayerMask.GetMask("Ground")))
                {
                    return PlayerManager.Instance.playerController;
                }
            }
            return null;
        }
        public override Controller GetTargetInView(Bounds bounds)
        {
            Bounds playerBound = PlayerManager.GetBoundPlayer();
            if (bounds.Intersects(playerBound))
            {
                return PlayerManager.Instance.playerController;
            }
            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            int f = core.movement.facingDirection;
            if (f == 0)
            {
                f = 1;
            }
            Vector2 os = new Vector2(offsetView.x * f, offsetView.y);
            Gizmos.DrawWireCube(transform.position + (Vector3)os, viewBounds);
            //Gizmos.DrawWireSphere(transform.position, sensorRange);
        }
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            HandleRotate();
        }
        void HandleRotate()
        {
            if (core.movement.facingDirection > 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 110, 0), 0.2f);
            }
            else if (core.movement.facingDirection < 0)
            {
                animatorHandle.transform.localRotation = Quaternion.Lerp(animatorHandle.transform.localRotation, Quaternion.Euler(0, 240, 0), 0.2f);
            }
        }

        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            healthBar.UpdateBar(NormalizeHealth());
            hitFX.Play();
            if (NormalizeHealth() <= 0)
            {
                animatorHandle.PlayAnimation("Dead", 0.1f, 1, true);
                core.movement.SetVelocity(new Vector2(damageInfo.hitDirection, 1), 15);
                Die(false);
            }
            if (NormalizeHealth() > 0 && damageInfo.stunTime > 0 && !isUnstopable)
            {
                animatorHandle.PlayAnimation("Stun", 0.1f, 1, true);
            }
        }

    }
}

