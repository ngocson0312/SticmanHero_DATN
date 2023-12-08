using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class TitanGroundSlamState : State
    {
        private BossTitan titan;
        private Controller target;
        private float timer;
        private int state;
        public TitanGroundSlamState(BossTitan controller, string stateName) : base(controller, stateName)
        {
            titan = controller;
            target = PlayerManager.Instance.playerController;
        }

        public override void EnterState()
        {
            timer = Random.Range(1f, 2f);
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
        }

        public override void UpdatePhysic()
        {

        }
        private void Move()
        {
            Vector2 dir = target.position - transform.position;
        }
    }
}
