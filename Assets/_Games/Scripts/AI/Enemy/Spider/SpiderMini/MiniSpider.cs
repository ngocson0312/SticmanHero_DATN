using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class MiniSpider : GroundEnemy
    {
        public SpiderMiniPatrolState spiderMiniPatrolState;
        public SpiderMiniChaseState spiderMiniChaseState;
        public SpiderMiniAttackState spiderMiniAttackState;
        public override void Initialize()
        {
            base.Initialize();
            spiderMiniPatrolState = new SpiderMiniPatrolState(this, "");
            spiderMiniChaseState = new SpiderMiniChaseState(this, "");
            spiderMiniAttackState = new SpiderMiniAttackState(this, "");
        }
        public override void ResetController()
        {
            base.ResetController();
            isActive = true;
            SwitchState(spiderMiniPatrolState);
        }
        public override void Die(bool deactiveCharacter)
        {
            //SoundManager.Instance.playSoundFx(//SoundManager.Instance.effSpiderDie);
            healthBar.Deactive();
            //animator.DeactiveCharacter();
            selfCollider.enabled = false;
            core.Deactive();
            // GameplayCtrl.Instance.freeEnemyBeKill(this, 2f);
            //GameplayCtrl.Instance.createCoin(transform.position + Vector3.up);
        }
    }

}
