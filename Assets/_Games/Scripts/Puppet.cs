using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Puppet : Enemy
    {
        public override Controller GetTargetInView()
        {
            return null;
        }
        public override Controller GetTargetInView(Bounds bounds)
        {
            throw new System.NotImplementedException();
        }
        private void Start()
        {
            core.Initialize(this);
        }
        private void Update()
        {
            if (runtimeStats.health < 0)
            {
               // Destroy(gameObject);
            }
             UpdateStatusEffects();
        }

    }
   
}
