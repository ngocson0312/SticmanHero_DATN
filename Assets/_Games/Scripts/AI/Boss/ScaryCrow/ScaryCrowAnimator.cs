using UnityEngine;
namespace SuperFight
{
    public class ScaryCrowAnimator : AnimatorHandle
    {
        public Vector2 slashDamageSize;
        public Vector2 slashDamageCenter;
        public Vector2 thrustDamageSize;
        public Vector2 thrustDamageCenter;
        public Vector2 chargeDamageSize;
        public Vector2 chargeDamageCenter;
        private ScaryCrow scaryCrow;
        public ParticleSystem groundSlamFx;
        public AudioClip swingSfx;
        public AudioClip groundCrashSfx;
        public AudioClip chargingSoundSfx;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
            scaryCrow = (ScaryCrow)controller;
        }
        public override void ResetAnimator()
        {

        }
        void Thrust()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.6f;
            damageInfo.stunForce = new Vector2(10, 0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(thrustDamageCenter.x * controller.core.movement.facingDirection, thrustDamageCenter.y);
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, thrustDamageSize, 0, scaryCrow.layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(colls[i]))
                {
                    colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void Slash(int type)
        {
            if (type != 0)
            {
                groundSlamFx.Play();
                CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
            }
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.3f;
            damageInfo.stunForce = new Vector2(3f, 0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(slashDamageCenter.x * controller.core.movement.facingDirection, slashDamageCenter.y); ;
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, slashDamageSize, 0, scaryCrow.layerTarget);
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
            damageInfo.damage = controller.runtimeStats.damage / 10;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.stunTime = 0.3f;
            damageInfo.stunForce = new Vector2(5f,0);
            damageInfo.hitDirection = controller.core.movement.facingDirection;
            Vector2 pos = controller.transform.position + new Vector3(chargeDamageCenter.x * controller.core.movement.facingDirection, chargeDamageCenter.y);
            Collider2D[] colls = Physics2D.OverlapBoxAll(pos, chargeDamageSize, 0, scaryCrow.layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponent<IDamage>() != null && !controller.core.combat.IsSelfCollider(colls[i]))
                {
                    colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }
        }
        void PlayChargingSound()
        {
            AudioManager.Instance.PlayOneShot(chargingSoundSfx, 1f);
        }
        void PlaySlashSound(int type)
        {
            if (type != 0)
            {
                AudioManager.Instance.PlayOneShot(groundCrashSfx, 1f);
            }
            else
            {
                AudioManager.Instance.PlayOneShot(swingSfx, 1f);
            }
        }
        void EnableRotate()
        {
            scaryCrow.rotateWhenAction = true;
        }
        void DisableRotate()
        {
            scaryCrow.rotateWhenAction = false;
        }
        private void OnDrawGizmosSelected()
        {
            Vector2 pos = transform.position;
            Gizmos.DrawWireCube(pos + slashDamageCenter, slashDamageSize);
            Gizmos.DrawWireCube(pos + thrustDamageCenter, thrustDamageSize);
            Gizmos.DrawWireCube(pos + chargeDamageCenter, chargeDamageSize);
        }
    }
}

