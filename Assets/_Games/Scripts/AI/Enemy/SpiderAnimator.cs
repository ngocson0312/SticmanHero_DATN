using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class SpiderAnimator : EnemyAnimator
    {
        public Transform arrowPrefab;
        public Transform firePoint;
        [SerializeField] Projectile projectile;
        public override void ResetAnimator()
        {
            gameObject.SetActive(true);
            ResumeAnimator();
            Animator.Rebind();
        }

        public void Attack()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = controller.runtimeStats.damage;
            damageInfo.owner = this.controller;
            damageInfo.characterType = controller.characterType;
            damageInfo.idSender = controller.core.combat.getColliderInstanceID;
            damageInfo.listEffect = new List<StatusEffectData>();
            damageInfo.listEffect.Add(new PoisonEffectData(controller, StatusEffectType.POISON));
            Projectile p = FactoryObject.Spawn<Projectile>("Projectile", projectile.transform);
            p.OnContact += (x) =>
            {
                for (int i = 0; i < x.Count; i++)
                {
                    x[i].TakeDamage(damageInfo);
                }
            };
            p.transform.position = transform.position + Vector3.up * 0.5f;
            p.Initialize(Vector2.down, damageInfo);

        }


    }
}
