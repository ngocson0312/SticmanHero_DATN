using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SuperFight
{
    public class Spider : GroundEnemy
    {
        public Enemy miniSpider;
        public ParticleSystem miniSpiderFx;
        public SpiderPatrolState spiderPatrolState;
        public SpiderChaseState spiderChaseState;
        public SpiderAttackState spiderAttackState;
        public override void Initialize()
        {
            base.Initialize();
            spiderPatrolState = new SpiderPatrolState(this, "");
            spiderChaseState = new SpiderChaseState(this, "");
            spiderAttackState = new SpiderAttackState(this, "");
        }
        public override void ResetController()
        {
            base.ResetController();
            isActive = true;
            SwitchState(spiderPatrolState);
        }
        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            healthBar.Deactive();
            // animator.DeactiveCharacter();
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effSpiderDie);
            miniSpiderFx.Play();

            Enemy s = Instantiate(miniSpider);
            CharacterStats characterStats = new CharacterStats(originalStats);
            characterStats.health /= 3;
            characterStats.damage /= 4;
            characterStats.exp /= 3;
            s.transform.SetParent(transform.parent);
            s.transform.position = effectDisplayHolder.position + Vector3.left * 1f;
            s.Initialize();
            s.Active();
            s.ConfigStats(characterStats, 0);

            s = Instantiate(miniSpider);
            s.transform.SetParent(transform.parent);
            s.transform.position = effectDisplayHolder.position + Vector3.right * 1f;
            s.Initialize();
            s.Active();
            s.ConfigStats(characterStats, 0);

            s = Instantiate(miniSpider);
            s.transform.SetParent(transform.parent);
            s.transform.position = effectDisplayHolder.position + Vector3.left * 2f;
            s.Initialize();
            s.Active();
            s.ConfigStats(characterStats, 0);

            s = Instantiate(miniSpider);
            s.transform.SetParent(transform.parent);
            s.transform.position = effectDisplayHolder.position + Vector3.right * 2f;
            s.Initialize();
            s.Active();
            s.ConfigStats(characterStats, 0);
        }
    }
}