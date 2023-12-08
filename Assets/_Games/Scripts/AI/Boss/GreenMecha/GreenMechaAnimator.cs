using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class GreenMechaAnimator : AnimatorHandle
    {
        private GreenMecha greenMecha;
        public Transform[] rocketPos;
        public Rocket rocketPrefab;
        public AudioClip rocketDeploySfx;
        public AudioClip[] footStepSfx;
        public AudioClip punchSfx;
        public AudioClip explosionSlamSfx;
        public ParticleSystem handFX;
        public Vector2 centerSlashCollider;
        public Vector2 sizeSlashCollider;
        public ParticleSystem groundSlamFX;
        public Vector2 centerGroundSlamCollider;
        public Vector2 sizeGroundSlamCollider;
        public GroundSpikes groundSpikes;
        private bool moveAnimator;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            greenMecha = (GreenMecha)controller;
        }
        public override void ResetAnimator()
        {

        }
        void Punch(int hand)
        {
            handFX.Play();
            CameraController.Instance.ShakeCamera(.25f, 0.5f, 5, 90, true);
            AudioManager.Instance.PlayOneShot(punchSfx, 0.5f);
            Vector3 point = controller.transform.position + new Vector3(centerSlashCollider.x * controller.core.movement.facingDirection, centerSlashCollider.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, sizeSlashCollider, 0, greenMecha.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.stunForce = new Vector2(5f, 0);
            damageInfo.stunTime = 0.3f;
            damageInfo.isKnockBack = true;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void ActiveGroundSpikes()
        {
            CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
            AudioManager.Instance.PlayOneShot(explosionSlamSfx, 0.5f);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.characterType = controller.characterType;
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.owner = controller;
            groundSpikes.Active(damageInfo);
        }
        void GroundSlam()
        {
            groundSlamFX.Play();
            CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
            AudioManager.Instance.PlayOneShot(explosionSlamSfx, 0.5f);
            Vector3 point = controller.transform.position + new Vector3(centerGroundSlamCollider.x * controller.core.movement.facingDirection, centerGroundSlamCollider.y, 0);
            Collider2D[] coll = Physics2D.OverlapBoxAll(point, sizeGroundSlamCollider, 0, greenMecha.layerTarget);
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            damageInfo.stunForce =  new Vector2(5f, 0);
            damageInfo.isKnockBack = true;
            damageInfo.stunTime = 0.3f;
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(coll[i]))
                {
                    coll[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        private void DeployRocket(int index)
        {
            Rocket r = Instantiate(rocketPrefab, rocketPos[index]);
            r.transform.localPosition = Vector3.zero;
            r.gameObject.SetActive(true);
            r.transform.parent = null;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.damage = controller.runtimeStats.damage / 2;
            if (index == 0)
            {
                AudioManager.Instance.PlayOneShot("missile_launch", 0.5f, 0.5f, null);
            }
            AudioManager.Instance.PlayOneShot(rocketDeploySfx, 0.5f);
            r.Initialize(rocketPos[index].up, damageInfo);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)centerSlashCollider, sizeSlashCollider);
            Gizmos.DrawWireCube(transform.position + (Vector3)centerGroundSlamCollider, sizeGroundSlamCollider);
        }
        void FootStep(int index)
        {
            CameraController.Instance.ShakeCamera(.25f, 0.5f, 5, 90, true);
            AudioManager.Instance.PlayOneShot(footStepSfx[index], 0.5f);
        }
        void EnableMoveOnAnimator()
        {
            moveAnimator = true;
        }
        void DisableMoveOnAnimator()
        {
            moveAnimator = false;
        }
        private void OnAnimatorMove()
        {
            if (moveAnimator)
            {
                controller.core.movement.SetVelocityX(5 * controller.core.movement.facingDirection);
            }
        }

    }
}
