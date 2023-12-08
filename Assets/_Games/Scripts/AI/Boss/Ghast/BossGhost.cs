using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class BossGhost : Boss
    {
        public Transform[] posMoves;
        public Transform laserPos;
        public Renderer renderObj;
        public ParticleSystem chargeFX;
        public ParticleSystem explosionFX;
        public ParticleSystem stunFX;
        public LineRenderer lineWarning;
        [Header("Roaming Stats")]
        public float flySpeed = 10f;
        public float angleBonus;
        [Header("Attack Stats")]
        public FireBall fireBall;
        public float attackRate;
        public PlayerManager player;
        public GhastRoamingState roamingState;
        public GhastThrowFireBallState throwFireBall;
        public GhastChargeState ghastChargeState;
        public GhastLaserState laserState;
        // public DamageCollider headDamage;
        public override void Initialize(BossFightArena bossFightArena)
        {
            base.Initialize(bossFightArena);
            player = PlayerManager.Instance;
            roamingState = new GhastRoamingState(this, "roaming");
            throwFireBall = new GhastThrowFireBallState(this, "throwfireball");
            ghastChargeState = new GhastChargeState(this, "charge");
            laserState = new GhastLaserState(this, "");
            SwitchState(roamingState);
            // headDamage.Initialize(this);
            core.movement.SetBodyType(RigidbodyType2D.Kinematic);
        }

        public void ThrowFireBall(Vector2 direction)
        {
            FireBall f = PathologicalGames.PoolManager.Pools["Projectile"].Spawn(fireBall.transform).GetComponent<FireBall>();
            f.transform.position = transform.position;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.listEffect = new List<StatusEffectData>();
            damageInfo.listEffect.Add(new BurningEffectData(this, StatusEffectType.BURNING));
            damageInfo.damage = runtimeStats.damage;
            damageInfo.idSender = core.combat.getColliderInstanceID;
            f.Initialize(direction, damageInfo);
            f.OnContact += (x) =>
            {
                for (int i = 0; i < x.Count; i++)
                {
                    x[i].TakeDamage(damageInfo);
                }
            };
        }
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            if (runtimeStats.health <= 0)
            {
                isActive = false;
                animatorHandle.PlayAnimation("Die", 0.1f, 1, true);
                core.movement.SetBodyType(RigidbodyType2D.Dynamic);
                Die(true);
            }
            else
            {
                animatorHandle.PlayAnimation("Hit", 0.1f, 1, true);
            }
            renderObj.material.DOColor(Color.red, 0.2f).OnComplete(() =>
            {
                renderObj.material.color = Color.white;
            });
        }
        public override void Active()
        {
            isActive = true;
        }
        public override void Resume()
        {
            base.Resume();
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        protected override void UpdatePhysic()
        {
        }

        protected override void UpdateLogic()
        {

        }
        private void OnDrawGizmos()
        {
            for (int i = 0; i < 12; i++)
            {
                float angle = i * (2 * Mathf.PI / 12);
                float x = Mathf.Cos(angle + angleBonus);
                float y = Mathf.Sin(angle + angleBonus);
                Gizmos.DrawRay(transform.position, new Vector3(x, y, 0) * 100);
            }
        }
    }
}

