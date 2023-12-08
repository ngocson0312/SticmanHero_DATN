using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class PigManDashState : State
    {
        private PigManSword pigman;
        public PigManDashState(PigManSword controller, string stateName) : base(controller, stateName)
        {
            pigman = controller;
        }

        public override void EnterState()
        {

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
    }
}
