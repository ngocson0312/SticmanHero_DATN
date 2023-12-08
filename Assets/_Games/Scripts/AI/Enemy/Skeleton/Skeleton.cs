using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Skeleton : GroundEnemy
    {
        public SkeletonPatrolState skeletonPatrolState;
        public SkeletonAttackState skeletonAttackState;
        

        public override void Initialize()
        {
            base.Initialize();
            skeletonPatrolState = new SkeletonPatrolState(this,"");
            skeletonAttackState = new SkeletonAttackState(this,"");
            isActive = true;
            SwitchState(skeletonPatrolState);
        }
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            animatorHandle.SetFloat("MoveAmount", core.movement.currentVelocity.x != 0 ? 1 : 0);
        }
    }
}
