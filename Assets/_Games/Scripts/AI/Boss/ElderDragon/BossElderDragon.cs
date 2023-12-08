using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class BossElderDragon : Boss
    {
        public ParticleSystem deathFX;
        public Transform[] posMoves;
        int currentPos;

        [Header("Flame")]
        public float rayLenght = 20f;
        public ParticleSystem flame;
        public Transform flamePosition;
        public LayerMask targetLayer;

        [Header("Roaming Stats")]
        public int direction = -1;
        public float flySpeed = 5f;
        public float speed = 10f;
        public PlayerManager player;

        public ElderRoamingState roamingState;
        public ElderMeleeSate meleeState;

        [Header("Attack Stats")]
        public FireBall fireBall;
        public float attackRate;
        [SerializeField] GameObject warning;

        public override void Initialize(BossFightArena bossFightArena)
        {
            base.Initialize(bossFightArena);
            player = PlayerManager.Instance;
            roamingState = new ElderRoamingState(this, "roaming");
            meleeState = new ElderMeleeSate(this, "meleeCombat");
            SwitchState(roamingState);
            DisableWarning();
        }
        public void ThrowFireBall(Vector2 direction)
        {
            animatorHandle.PlayAnimation("Attack1", 0.1f, 1, true);
            FireBall f = PathologicalGames.PoolManager.Pools["Projectile"].Spawn(fireBall.transform).GetComponent<FireBall>();
            f.transform.position = flamePosition.position;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = runtimeStats.damage;
            damageInfo.idSender = core.combat.getColliderInstanceID;
            f.Initialize(direction, damageInfo);
        }

        public void ActiveWarning()
        {
            warning.SetActive(true);
        }

        public void DisableWarning()
        {
            warning.SetActive(false);
            CameraController.Instance.ShakeCamera(.35f, 0.7f, 10, 90, true);
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
        protected override void UpdateLogic()
        {
            if (!isActive) return;
            isInteracting = animatorHandle.GetBool("IsInteracting");
        }

        protected override void UpdatePhysic()
        {

        }


    }
}
