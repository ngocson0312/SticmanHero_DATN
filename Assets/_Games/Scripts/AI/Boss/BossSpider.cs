using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class BossSpider : Boss
    {
        public float stateTimer;
        public int state; //1 normal attack,2 farattack,3 deploy kid
        int currentStateAtk;
        public ParticleSystem fxBlood;
        [Header("Movement")]
        float moveAmount;
        public int direction;
        public float speed = 5f;
        [Header("CloseRange")]
        float attackTimer;
        public float attackCloseRange = 3f;
        [Header("FarRange")]
        public Transform firePoint;
        public float fireRate = 1f;
        float fireRateTimer;
        public FireBall acidProjectile;
        [Header("SpawnState")]
        public ParticleSystem[] posSpawn;
        public MiniSpider spider;
        int countSpawn;
        float spawnTimer;
        bool bosscanattack = true;
        public override void Initialize(BossFightArena bossFightArena)
        {
            base.Initialize(bossFightArena);
            stateTimer = 1f;
            bosscanattack = true;
        }

        void SwitchState(int state)
        {
            currentStateAtk++;
            if (currentStateAtk > 3)
            {
                currentStateAtk = 1;
            }
            this.state = currentStateAtk;
            countSpawn = Random.Range(2, 4);
            spawnTimer = 0.01f;
            fireRateTimer = fireRate;
            attackTimer = 0.01f;
        }
        void AttackFarRange()
        {
            Vector2 playerPosition = PlayerManager.Instance.transform.position;
            Vector2 dir = playerPosition - (Vector2)transform.position;
            core.movement.SetVelocityX(0);
            moveAmount = 0;
            if (dir.x < 0)
            {
                direction = -1;
                animatorHandle.transform.rotation = Quaternion.Lerp(animatorHandle.transform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
            }
            else if (dir.x > 0)
            {
                direction = 1;
                animatorHandle.transform.rotation = Quaternion.Lerp(animatorHandle.transform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
            }
            if (fireRateTimer > 0)
            {
                fireRateTimer -= Time.deltaTime;
                if (fireRateTimer <= 0)
                {
                    animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);
                    FireBall fb = PathologicalGames.PoolManager.Pools["Projectile"].Spawn(acidProjectile.transform).GetComponent<FireBall>();
                    fb.transform.position = firePoint.position;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.idSender = core.combat.getColliderInstanceID;
                    damageInfo.damage = runtimeStats.damage / 2;
                    damageInfo.listEffect = new List<StatusEffectData>();
                    damageInfo.listEffect.Add(new PoisonEffectData(this, StatusEffectType.POISON));
                    fb.OnContact += (x) =>
                    {
                        for (int i = 0; i < x.Count; i++)
                        {
                            x[i].TakeDamage(damageInfo);
                        }
                    };
                    fb.Initialize(Vector2.right * direction, damageInfo);
                    fireRateTimer = fireRate;

                }
            }
        }
        void ChasePlayer()
        {
            Vector2 playerPosition = PlayerManager.Instance.transform.position;
            if (Vector2.Distance(transform.position, playerPosition) > attackCloseRange)
            {
                Vector2 dir = playerPosition - (Vector2)transform.position;
                if (dir.x < 0)
                {
                    direction = -1;
                    animatorHandle.transform.rotation = Quaternion.Lerp(animatorHandle.transform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
                }
                else if (dir.x > 0)
                {
                    direction = 1;
                    animatorHandle.transform.rotation = Quaternion.Lerp(animatorHandle.transform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
                }
                core.movement.SetVelocityX(direction * speed);
                moveAmount = 1;
            }
            else
            {
                moveAmount = 0;
                core.movement.SetVelocityX(0);
                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);
                        attackTimer = -0.001f;
                    }
                }
                if (attackTimer < 0)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= -0.3f && attackTimer > -0.35f)
                    {
                        float distance = Vector2.Distance(transform.position, playerPosition);
                        float dot = Vector2.Dot((playerPosition - (Vector2)transform.position).normalized, Vector2.right * direction);
                        if (distance <= attackCloseRange && dot > 0)
                        {
                            DamageInfo damageInfo = new DamageInfo();
                            damageInfo.damage = runtimeStats.damage;
                            damageInfo.owner = this;
                            damageInfo.listEffect = new List<StatusEffectData>();
                            damageInfo.listEffect.Add(new PoisonEffectData(this, StatusEffectType.POISON));
                            PlayerManager.Instance.playerController.core.combat.TakeDamage(damageInfo);
                        }
                        attackTimer = -0.36f;
                    }
                    if (attackTimer <= -1f)
                    {
                        attackTimer = 1;
                    }
                }
            }
        }
        void DeploySpider()
        {
            core.movement.SetVelocityX(0);
            moveAmount = 0;
            int indexPosSpawn = Random.Range(0, posSpawn.Length);
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0 && countSpawn > 0)
                {
                    stateTimer = 3;
                    MiniSpider c = Instantiate(spider);
                    c.transform.SetParent(arena.transform);
                    posSpawn[indexPosSpawn].Play();
                    c.transform.position = posSpawn[indexPosSpawn].transform.position;
                    CharacterStats characterStats = new CharacterStats();
                    characterStats.health = (int)(originalStats.health / 20f);
                    characterStats.damage = (int)(originalStats.damage / 10f);
                    c.Initialize();
                    c.ConfigStats(characterStats, 0);
                    c.Active();
                    spawnTimer = 0.5f;
                    countSpawn--;
                }
            }
        }

        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!isActive) return;
            base.OnTakeDamage(damageInfo);
            fxBlood.Play();
            if (runtimeStats.health <= 0)
            {
                isActive = false;
                animatorHandle.PlayAnimation("Dead", 0.1f, 1, false);
                bosscanattack = false;
                Die(true);
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, attackCloseRange);
        }

        public override void Active()
        {
            isActive = true;
        }

        protected override void UpdateLogic()
        {
            if (stateTimer > 0)
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    SwitchState(Random.Range(1, 3));
                    stateTimer = Random.Range(3f, 4f);
                }
            }
            if (bosscanattack)
            {
                switch (state)
                {
                    case 1:
                        ChasePlayer();
                        break;
                    case 2:
                        AttackFarRange();
                        break;
                    case 3:
                        DeploySpider();
                        break;
                }
                animatorHandle.SetFloat("MoveAmount", moveAmount);
            }
        }

        protected override void UpdatePhysic()
        {
        }
    }
}

