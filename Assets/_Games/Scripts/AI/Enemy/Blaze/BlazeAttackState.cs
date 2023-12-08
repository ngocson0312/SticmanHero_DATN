using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class BlazeAttackState : State
    {
        Blaze enemy;
        bool isFireBall = false;
        float attackTimer = 0;
        int currentDirection;
        public BlazeAttackState(Blaze controller, string stateName) : base(controller, stateName)
        {
            this.enemy = controller;
            enemy.animatorHandle.OnEventAnimation += OnEvent;
        }

        ~BlazeAttackState()
        {
            enemy.animatorHandle.OnEventAnimation -= OnEvent;
        }

        private void OnEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                enemy.CiticalVfx.Play();

            }

            if (eventName.Equals("FireBall"))
            {
                Controller target = enemy.GetTargetInView();
                if (target == null)
                {
                    enemy.SwitchState(enemy.BlazePatrolState);
                    return;
                }
                FireBall f = PathologicalGames.PoolManager.Pools["Projectile"].Spawn(enemy.fireBall.transform).GetComponent<FireBall>();
                f.transform.position = enemy.PosFire.position;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.listEffect = new List<StatusEffectData>();
                damageInfo.listEffect.Add(new BurningEffectData(controller, StatusEffectType.BURNING));
                damageInfo.damage = enemy.runtimeStats.damage;
                damageInfo.stunTime = 0.1f;
                damageInfo.idSender = core.combat.getColliderInstanceID;
                Vector2 dirr = target.transform.position - controller.transform.position;
                f.OnContact += (x) =>
                {
                    for (int i = 0; i < x.Count; i++)
                    {
                        x[i].TakeDamage(damageInfo);
                    }
                };
                f.Initialize(dirr, damageInfo);
            }
        }
        public override void EnterState()
        {
            currentDirection = enemy.core.movement.facingDirection;
            attackTimer = Random.Range(0, 0.3f);
        }
        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
            Controller target = enemy.GetTargetInView();
            enemy.core.movement.SetVelocityX(0);
            if (target == null)
            {
                enemy.SwitchState(enemy.BlazePatrolState);
                return;
            }
            if (Vector2.SqrMagnitude(target.transform.position - controller.transform.position) > enemy.attackRange && !controller.isInteracting)
            {
                enemy.SwitchState(enemy.BlazeChaseState);
                return;
            }
            if (!controller.isInteracting)
            {
                Vector2 dir = target.transform.position - controller.transform.position;
                if (dir.x < 0)
                {
                    currentDirection = -1;
                }
                else if (dir.x > 0)
                {
                    currentDirection = 1;
                }
                if (!controller.isInteracting)
                {
                    if (currentDirection != controller.core.movement.facingDirection)
                    {
                        controller.core.movement.Flip();

                    }
                }
                float distance = Vector2.Distance(transform.position, target.position);
                Vector2 dirr = target.transform.position - controller.transform.position;
                if (attackTimer >= 0)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        enemy.animatorHandle.PlayAnimation("Attack", 0.1f, 1, true);
                        attackTimer = 2 / enemy.attackSpeed;
                    }
                }
            }
        }



        public override void UpdatePhysic()
        {

        }
    }
}
