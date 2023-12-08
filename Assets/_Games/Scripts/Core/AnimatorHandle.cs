using UnityEngine;
using System;
using System.Collections.Generic;

namespace SuperFight
{
    [RequireComponent(typeof(Animator))]
    public abstract class AnimatorHandle : MonoBehaviour
    {
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }
        private Animator _animator;
        protected Controller controller;
        public event System.Action<string> OnEventAnimation;
        public event System.Action<Vector3> OnAnimatorUpdate;
        private Dictionary<string, int> layers = new Dictionary<string, int>();
        public virtual void Initialize(Controller controller)
        {
            _animator = GetComponent<Animator>();
            this.controller = controller;
            ResetAnimator();
        }
        public virtual void ResetAnimator()
        {
            ResumeAnimator();
        }
        public virtual void PauseAnimator()
        {
            Animator.speed = 0;
        }
        public virtual void ResumeAnimator()
        {
            Animator.speed = 1;
        }
        public void SetFloat(string parameter, float value)
        {
            Animator.SetFloat(parameter, value);
        }
        public void SetBool(string parameter, bool status)
        {
            Animator.SetBool(parameter, status);
        }
        public bool GetBool(string param)
        {
            return Animator.GetBool(param);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, int layer)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, layer);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, int layer, bool isInteracting)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, layer);
            Animator.SetBool("IsInteracting", isInteracting);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, int layer, bool isInteracting, bool isApplyRootMotion)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, layer);
            Animator.SetBool("IsInteracting", isInteracting);
            Animator.SetBool("IsApplyRootMotion", isApplyRootMotion);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, string layerName, bool isInteracting)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, GetLayer(layerName));
            Animator.SetBool("IsInteracting", isInteracting);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, string layerName, bool isInteracting, bool isApplyRootMotion)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, GetLayer(layerName));
            Animator.SetBool("IsInteracting", isInteracting);
            Animator.SetBool("IsApplyRootMotion", isInteracting);
        }
        public void PlayAnimation(string stateName, float normalizedTransitionDuration, int layer, bool isInteracting, float speedAnimation)
        {
            Animator.CrossFade(stateName, normalizedTransitionDuration, layer);
            Animator.SetBool("IsInteracting", isInteracting);
            Animator.speed = speedAnimation;
        }
        public int GetLayer(string layerName)
        {
            int layer = 0;
            if (layers.ContainsKey(layerName))
            {
                layer = layers[layerName];
            }
            else
            {
                layer = Animator.GetLayerIndex(layerName);
                layers.Add(layerName, layer);
            }
            return layer;
        }
        public void SendEvent(string eventName)
        {
            OnEventAnimation?.Invoke(eventName);
        }
        public virtual void DeactiveCharacter()
        {
            gameObject.SetActive(false);
        }
        private void OnAnimatorMove()
        {
            OnAnimatorUpdate?.Invoke(Animator.deltaPosition);
        }
        public Transform GetBone(HumanBodyBones bone)
        {
            return Animator.GetBoneTransform(bone);
        }
    }

}
