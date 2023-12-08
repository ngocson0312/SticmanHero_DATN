using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class EnemyAnimator : AnimatorHandle
    {
        public override void ResetAnimator()
        {
            gameObject.SetActive(true);
            base.ResetAnimator();
            Animator.Rebind();
        }
    }
}

