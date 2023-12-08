using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class RenekAnimator : AnimatorHandle
    {
        public Vector2 centerSlashCollider;
        public Vector2 sizeSlashCollider;
        public Vector2 centerChargingCollider;
        public Vector2 sizeChargingCollider;
        public AudioClip dashSfx;
        public AudioClip spinSfx;
        private Renek renek;
        private List<Collider2D> contact;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            renek = (Renek)controller;
        }
        public override void ResetAnimator()
        {

        }
        void Slash()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.3f;
            damageInfo.stunForce = new Vector2(2, 0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(centerSlashCollider.x * controller.core.movement.facingDirection, centerSlashCollider.y);
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, centerSlashCollider, 0, renek.layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(colls[i]))
                {
                    colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void HeavySlash()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.6f;
            damageInfo.stunForce = new Vector2(10, 0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(centerSlashCollider.x * controller.core.movement.facingDirection, centerSlashCollider.y);
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, centerSlashCollider, 0, renek.layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(colls[i]))
                {
                    colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        public void ChargeCollider()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.6f;
            damageInfo.stunForce = new Vector2(10, 0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(centerChargingCollider.x * controller.core.movement.facingDirection, centerChargingCollider.y);
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, sizeChargingCollider, 0, renek.layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (!contact.Contains(colls[i]))
                {
                    if (colls[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(colls[i]))
                    {
                        colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                        contact.Add(colls[i]);
                    }
                }
            }
        }
        void OnCharging()
        {
            AudioManager.Instance.PlayOneShot(dashSfx, 1f);
        }
        void OnSpin()
        {
            AudioManager.Instance.PlayOneShot(spinSfx, 1f);
        }
        public void StartCharging()
        {
            contact = new List<Collider2D>();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 pos = transform.position;
            Gizmos.DrawWireCube(pos + centerSlashCollider, sizeSlashCollider);
            Gizmos.DrawWireCube(pos + centerChargingCollider, sizeChargingCollider);
        }
    }
}

