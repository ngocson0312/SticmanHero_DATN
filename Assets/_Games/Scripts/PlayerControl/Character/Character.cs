using Spine.Unity;
using UnityEngine;
namespace SuperFight
{
    public class Character : AnimatorHandle
    {
        public Transform weaponHolderLeft;
        public Transform weaponHolderRight;
        public GameObject[] cloths;
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);
        }
      
        public void PlayAnimation(string stateName, int layer, bool isInteracting)
        {
            PlayAnimation(stateName, 0.1f, layer);
            Animator.SetBool("IsInteracting", isInteracting);
        }
        public override void ResetAnimator()
        {
            gameObject.SetActive(true);
            Animator.Rebind();
            ResumeAnimator();
        }
    }
}

