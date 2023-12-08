using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class ExecutionerAnimator : AnimatorHandle
    {
        public ExecutionerAxe[] axes;
        public GameObject[] realAxes;
        public Vector2 groundSlashSize;
        public Vector2 groundSlashPoint;
        public Vector2 slashPoint;
        public Vector2 slashSize;
        public ParticleSystem groundSlamFX;
        public AudioClip iceExplosionSfx;
        public AudioClip axeSwoosh;
        public AudioClip flySwoosh;
        private Executioner executioner;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            executioner = (Executioner)controller;
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].CatchAxe();
            }
        }
        void FlyUp()
        {
            AudioManager.Instance.PlayOneShot(flySwoosh, 0.8f);
        }

        void ThrowAxes()
        {
            for (int i = 0; i < realAxes.Length; i++)
            {
                realAxes[i].SetActive(false);
            }
            Vector3 direction = new Vector3(controller.core.movement.facingDirection, 0);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage / 2;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].ThrowAxe(damageInfo);
            }
        }
        void CatchAxes()
        {
            for (int i = 0; i < realAxes.Length; i++)
            {
                realAxes[i].SetActive(true);
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].CatchAxe();
            }
        }
        void GroundSlash()
        {
            CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
            AudioManager.Instance.PlayOneShot(iceExplosionSfx, 0.8f);
            groundSlamFX.Play();
            Vector3 direction = new Vector3(controller.core.movement.facingDirection, 0);
            Vector3 point = controller.transform.position + new Vector3(groundSlashPoint.x * direction.x, groundSlashPoint.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, groundSlashSize, 0, executioner.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.stunForce = new Vector2(7f, 0);
            damageInfo.stunTime = 0.3f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void GroundSlashWall()
        {

        }
        void EnableRotate()
        {
            executioner.rotateWhenAction = true;
        }
        void DeactiveRotate()
        {
            executioner.rotateWhenAction = false;
        }
        void ActiveAxe()
        {
            AudioManager.Instance.PlayOneShot(axeSwoosh, 0.8f);
        }
        void Slash()
        {
            Vector3 direction = new Vector3(controller.core.movement.facingDirection, 0);
            Vector3 point = controller.transform.position + new Vector3(slashPoint.x * direction.x, slashPoint.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, slashSize, 0, executioner.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage / 2;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.stunForce = new Vector2(3f, 0);
            damageInfo.stunTime = 0.3f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }

        public override void ResetAnimator()
        {

        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)groundSlashPoint, groundSlashSize);
            Gizmos.DrawWireCube(transform.position + (Vector3)slashPoint, slashSize);
        }
    }
}

