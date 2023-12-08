using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Combat : CoreComponent, IDamage
    {
        private float stunTimer;
        public CharacterType getType
        {
            get => core.controller.characterType;
        }
        public int getColliderInstanceID
        {
            get { return GetComponent<Collider2D>().GetInstanceID(); }
        }

        public Controller controller => core.controller;

        public bool IsSelfCollider(Collider2D other)
        {
            return GetComponent<Collider2D>().Equals(other);
        }
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (damageInfo.characterType == getType)
            {
                damageInfo.onHitSuccess?.Invoke(false);
                return;
            }
            if (core.controller.GetInstanceID() == damageInfo.idSender || core.controller.isInvincible)
            {
                damageInfo.onHitSuccess?.Invoke(false);
                return;
            }
            AudioManager.Instance.PlayOneShot(damageInfo.impactSound, 1f);
            if (controller.characterType != CharacterType.Boss)
            {
                core.controller.isStunning = true;
                stunTimer = damageInfo.stunTime;
                damageInfo.stunForce.x *= damageInfo.hitDirection;
                core.movement.SetVelocity(damageInfo.stunForce);
            }
            damageInfo.onHitSuccess?.Invoke(true);
            core.controller.OnTakeDamage(damageInfo);
        }
        public void UpdateLogic()
        {
            if (stunTimer > 0)
            {
                if (core.collisionSenses.IsTouchWallBehind())
                {
                    stunTimer -= Time.deltaTime * 3;
                }
                else
                {
                    stunTimer -= Time.deltaTime;
                }
            }
            else
            {
                core.controller.isStunning = false;
            }
        }
    }
}

