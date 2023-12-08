using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class HornetRequiemAnimator : AnimatorHandle
    {
        private HornetRequiem hornetRequiem;
        public ParticleSystem trails;
        public Vector2 centerSlash;
        public Vector2 sizeSlash;
        public Vector2 centerGroundSlam;
        public Vector2 sizeGroundSlam;
        public float explosionRange;
        public Vector2 explosionPoint;
        public float circleRange;
        public Vector2 circlePoint;
        public ParticleSystem groundSlamFX;
        public ParticleSystem explosionFX;
        public ParticleSystem chargeExlosionFX;
        public ParticleSystem slashFX;
        public AudioClip groundCrash;
        public AudioClip swordSwing;
        public AudioClip dashSound;
        public AudioClip flySound;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            hornetRequiem = (HornetRequiem)controller;
        }
        public override void ResetAnimator()
        {
            DisableTrail();
        }
        void Slash()
        {
            AudioManager.Instance.PlayOneShot(swordSwing, 1);
            int direction = controller.core.movement.facingDirection;
            Vector3 point = controller.transform.position + new Vector3(centerSlash.x * direction, centerSlash.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, sizeSlash, 0, hornetRequiem.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = direction;
            damageInfo.stunForce = new Vector2(5, 0);
            damageInfo.stunTime = 0.3f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void CircleSlash()
        {
            slashFX.Play();
            AudioManager.Instance.PlayOneShot(swordSwing, 1);
            int direction = controller.core.movement.facingDirection;
            Vector3 point = controller.transform.position + new Vector3(circlePoint.x * direction, circlePoint.y, 0);
            Collider2D[] coll = Physics2D.OverlapCircleAll(point, circleRange, hornetRequiem.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = direction;
            damageInfo.stunForce = new Vector2(15, 0);
            damageInfo.stunTime = 0.55f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void GroundSlam()
        {
            chargeExlosionFX.Play();
            groundSlamFX.Play();
            AudioManager.Instance.PlayOneShot(groundCrash, 1);
            int direction = controller.core.movement.facingDirection;
            Vector3 point = controller.transform.position + new Vector3(centerGroundSlam.x * direction, centerGroundSlam.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, sizeGroundSlam, 0, hornetRequiem.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = direction;
            damageInfo.stunForce = new Vector2(5, 0);
            damageInfo.stunTime = 0.51f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
            Invoke(nameof(Explosion), 1f);
        }
        void Explosion()
        {
            AudioManager.Instance.PlayOneShot("Explosion", 1);
            chargeExlosionFX.Stop();
            explosionFX.Play();
            int direction = controller.core.movement.facingDirection;
            Vector3 point = controller.transform.position + new Vector3(explosionPoint.x * direction, explosionPoint.y, 0);
            Collider2D[] coll = Physics2D.OverlapCircleAll(point, explosionRange, hornetRequiem.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = direction;
            damageInfo.stunForce = new Vector2(15, 0);
            damageInfo.stunTime = 0.51f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void FlyUp()
        {
            AudioManager.Instance.PlayOneShot(flySound, 0.7f);
        }
        void Dash()
        {
            AudioManager.Instance.PlayOneShot(dashSound, 1);
        }
        void EnableTrail()
        {
            trails.Play();
        }
        void DisableTrail()
        {
            trails.Stop();
        }
        void EnableRotate()
        {
            hornetRequiem.rotateWhenAction = true;
        }
        void DeactiveRotate()
        {
            hornetRequiem.rotateWhenAction = false;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)centerSlash, sizeSlash);
            Gizmos.DrawWireCube(transform.position + (Vector3)centerGroundSlam, sizeGroundSlam);
            Gizmos.DrawWireSphere(transform.position + (Vector3)explosionPoint, explosionRange);
            Gizmos.DrawWireSphere(transform.position + (Vector3)circlePoint, circleRange);
        }
    }

}
