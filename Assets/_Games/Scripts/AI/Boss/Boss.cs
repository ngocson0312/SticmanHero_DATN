using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public abstract class Boss : Controller
    {
        protected BossFightArena arena;
        protected State currentState;
        protected Queue<AIAction> queueAction = new Queue<AIAction>();
        public event Action<Boss> OnDisappear;
        public ParticleSystem fadeOutFX;
        public ParticleSystem deathExplosionFX;
        public Shader fadeShader;
        public virtual void Initialize(BossFightArena arena)
        {
            this.arena = arena;
            characterType = CharacterType.Boss;
            isActive = false;
            core.Initialize(this);
            animatorHandle.Initialize(this);
            runtimeStats = new CharacterStats(originalStats);
        }
        public abstract void Active();
        private void Update()
        {
            if (!isActive) return;
            core.UpdateLogicCore();
            if (currentState != null)
            {
                currentState.UpdateLogic();
            }
            if (queueAction.Count > 0)
            {
                queueAction.Peek().UpdateLogic(Time.deltaTime);
            }
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].UpdateEffect();
            }
            UpdateLogic();
        }
        private void FixedUpdate()
        {
            if (!isActive) return;
            if (currentState != null)
            {
                currentState.UpdatePhysic();
            }
            if (queueAction.Count > 0)
            {
                queueAction.Peek().UpdatePhysic(Time.fixedDeltaTime);
            }
            UpdatePhysic();
        }
        public void SwitchState(State newState)
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }
            currentState = newState;
            currentState.EnterState();
        }
        public void AddAction(AIAction newAction)
        {
            if (queueAction.Count == 0)
            {
                newAction.OnActionEnter();
            }
            queueAction.Enqueue(newAction);
        }
        public void FinishAction()
        {
            queueAction.Dequeue();
            if (queueAction.Count > 0)
            {
                queueAction.Peek().OnActionEnter();
            }
        }
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            arena.OnBossTakeDamage(damageInfo.damage, this);
            runtimeStats.health -= damageInfo.damage;
            if (damageInfo.listEffect != null)
            {
                for (int i = 0; i < damageInfo.listEffect.Count; i++)
                {
                    AddStatusEffect(damageInfo.listEffect[i]);
                }
            }
            base.OnTakeDamage(damageInfo);
        }
        protected abstract void UpdateLogic();
        protected abstract void UpdatePhysic();
        public override void Die(bool deactiveCharacter)
        {
            base.Die(deactiveCharacter);
            core.movement.SetVelocityZero();
            core.Deactive();
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].OnFinishEffect();
            }
            GameManager.Instance.UpdateTask(QuestType.KILL_BOSS, 1);
            StartCoroutine(IEDie());
        }
        IEnumerator IEDie()
        {
            fadeOutFX.Play();
            SkinnedMeshRenderer[] skinnedMeshes = animatorHandle.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < skinnedMeshes.Length; i++)
            {
                skinnedMeshes[i].material.shader = fadeShader;
                skinnedMeshes[i].material.SetFloat("_Opacity", 1);
            }
            float v = 2f;
            AudioManager.Instance.PlayOneShot("OnBossDefeat", 1);
            while (v > 0)
            {
                for (int i = 0; i < skinnedMeshes.Length; i++)
                {
                    skinnedMeshes[i].material.SetFloat("_Opacity", v);
                }
                v -= Time.deltaTime / 2f;
                yield return null;
            }
            AudioManager.Instance.PlayOneShot("BossDead", 1);
            fadeOutFX.Stop();
            deathExplosionFX.Play();
            animatorHandle.DeactiveCharacter();
            Disappear();
        }
        protected void Disappear()
        {
            OnDisappear?.Invoke(this);
        }
    }
}

