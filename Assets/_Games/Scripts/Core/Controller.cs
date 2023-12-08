using System.Collections;
using System.Collections.Generic;
using SuperFight;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
namespace SuperFight
{
    public abstract class Controller : MonoBehaviour
    {
        public Core core;
        public AnimatorHandle animatorHandle;
        public CharacterType characterType;
        public CharacterStats originalStats;
        public CharacterStats runtimeStats;
        public Vector3 position
        {
            get { return transform.position; }
        }
        public List<StatusEffect> statusEffects = new List<StatusEffect>();
        public static event Action<Controller> OnDead;
        public event Action OnCrit;
        public Transform effectDisplayHolder;
        public float controllerSpeed { get; protected set; }
        public bool isInteracting;
        public bool isApplyRootMotion;
        public bool isStunning;
        public bool isInvincible;
        public bool isActive;
        public int deathDenyCount;
        public virtual void OnTakeDamage(DamageInfo damageInfo)
        {
            string content = "";
            if (damageInfo.damageType == DamageType.NORMAL)
            {
                content = "<color=white>" + damageInfo.damage.ToString() + "</color>";
            }
            if (damageInfo.damageType == DamageType.CRITICAL)
            {
                content = "<color=red>" + damageInfo.damage.ToString() + "</color>";
            }
            if (damageInfo.damageType == DamageType.TRUEDAMAGE)
            {
                content = "<color=#ff7f50>" + damageInfo.damage.ToString() + "</color>";
            }
            UltimateTextDamageManager.Instance.Add(content, effectDisplayHolder.position);
        }
        public virtual void Die(bool deactiveCharacter)
        {
            if (!isActive) return;
            isActive = false;
            OnDead?.Invoke(this);
            if (deactiveCharacter)
            {
                animatorHandle.DeactiveCharacter();
            }
        }
        public virtual void ResetController()
        {
            Resume();
            core.Active();
            animatorHandle.ResetAnimator();
            SetControllerSpeed(1f);
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].OnFinishEffect();
            }
            statusEffects.Clear();
        }
        public virtual void SetControllerSpeed(float speed)
        {
            animatorHandle.Animator.speed = speed;
            controllerSpeed = speed;
            if (controllerSpeed < 0)
            {
                controllerSpeed = 0;
            }
        }
        public virtual void SetHealth(int amount)
        {
            if (amount > originalStats.health)
            {
                amount = originalStats.health;
            }
            runtimeStats.health = amount;
        }
        public void AddDenyDeathCount()
        {
            deathDenyCount++;
        }
        public void RemoveDenyDeathCount()
        {
            deathDenyCount--;
        }
        public virtual void AddStatusEffect(StatusEffectData statusEffectData)
        {
            switch (statusEffectData.effectType)
            {
                case StatusEffectType.BURNING:
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            statusEffects[i].OnStartEffect(this);
                            return;
                        }
                    }
                    break;
                case StatusEffectType.FROSTBITE:
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            statusEffects[i].OnStartEffect(this);
                            FrostbiteEffectData data = statusEffectData as FrostbiteEffectData;
                            FrostbiteEffect frostbiteEffect = statusEffects[i] as FrostbiteEffect;
                            if (!frostbiteEffect.isFreerzing)
                            {
                                frostbiteEffect.frozenPoint += data.freezePoint;
                                if (frostbiteEffect.frozenPoint >= 100)
                                {
                                    frostbiteEffect.Freeze();
                                }
                            }
                            return;
                        }
                    }
                    break;
                case StatusEffectType.ELECTROCUTE:
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            statusEffects[i].OnStartEffect(this);
                            return;
                        }
                    }
                    break;
                case StatusEffectType.BLEEDING:
                    List<BleedingEffect> listBleed = new List<BleedingEffect>();
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            listBleed.Add(statusEffects[i] as BleedingEffect);
                        }
                    }
                    if (listBleed.Count >= 4)
                    {
                        BleedingEffect bleedingEffect = StatusEffectManager.Instance.CreateEffect(statusEffectData) as BleedingEffect;
                        bleedingEffect.transform.parent = transform;
                        bleedingEffect.transform.localPosition = Vector3.zero;
                        bleedingEffect.OnStartEffect(this);
                        bleedingEffect.BleedingTarget();
                        for (int i = 0; i < listBleed.Count; i++)
                        {
                            listBleed[i].OnFinishEffect();
                        }
                        return;
                    }
                    break;
                case StatusEffectType.POISON:
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            statusEffects[i].OnStartEffect(this);
                            return;
                        }
                    }
                    break;
                case StatusEffectType.CURSE:
                    for (int i = 0; i < statusEffects.Count; i++)
                    {
                        if (statusEffects[i].statusName == statusEffectData.effectType)
                        {
                            statusEffects[i].OnStartEffect(this);
                            var curseEffect = statusEffects[i] as CurseEffect;
                            curseEffect.cursePointAdd++;
                            return;
                        }
                    }
                    break;
            }
            StatusEffect statusEffect = StatusEffectManager.Instance.CreateEffect(statusEffectData);
            statusEffect.OnStartEffect(this);
            statusEffect.OnComplete += () => RemoveEffect(statusEffect);
            statusEffect.transform.parent = transform;
            statusEffect.transform.localPosition = Vector3.zero;
            statusEffects.Add(statusEffect);
            float xPos = 0;
            if (statusEffects.Count % 2 == 0 && statusEffects.Count > 1)
            {
                xPos = -(statusEffects.Count / 2 * 0.25f) + (0.25f / 2f);
            }
            else
            {
                if (statusEffects.Count > 1)
                {
                    xPos = (-statusEffects.Count / 2) * 0.25f;
                }
            }
            float yPos = effectDisplayHolder.localPosition.y;
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].iconEffect.localPosition = new Vector3(xPos, yPos, 0);
                xPos += 0.25f;
            }
        }
        private void RemoveEffect(StatusEffect statusEffect)
        {
            statusEffect.iconEffect.gameObject.SetActive(false);
            statusEffects.Remove(statusEffect);
            float xPos = 0;
            if (statusEffects.Count % 2 == 0 && statusEffects.Count > 1)
            {
                xPos = -(statusEffects.Count / 2 * 0.25f) + (0.25f / 2f);
            }
            else
            {
                if (statusEffects.Count > 1)
                {
                    xPos = (-statusEffects.Count / 2) * 0.25f;
                }
            }
            float yPos = effectDisplayHolder.localPosition.y;
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].iconEffect.localPosition = new Vector3(xPos, yPos, 0);
                xPos += 0.25f;
            }
        }
        public void UpdateStatusEffects()
        {
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].UpdateEffect();
            }
        }
        public float NormalizeHealth()
        {
            return (float)runtimeStats.health / (float)originalStats.health;
        }
        public virtual void Pause()
        {
            isActive = false;
            animatorHandle.PauseAnimator();
            core.Pause();
        }
        public virtual void Resume()
        {
            if (runtimeStats.health > 0)
            {
                isActive = true;
            }
            animatorHandle.ResumeAnimator();
            core.Resume();
        }
        public bool IsCrit()
        {
            bool isCrit = Random.Range(0, 100) < runtimeStats.critRate;
            if (isCrit)
            {
                OnCrit?.Invoke();
            }
            return isCrit;
        }
        public virtual void AddHealth(int amount)
        {
            runtimeStats.health += amount;
            if (runtimeStats.health > originalStats.health)
            {
                runtimeStats.health = originalStats.health;
            }
            string content = "<color=green>+" + amount + "</color>";
            UltimateTextDamageManager.Instance.Add(content, effectDisplayHolder.position);
        }
        public void lifeStealing(int damage) // he thong hut mau
        {
            int hp = (int)(damage * runtimeStats.lifeSteal / 100f);
            AddHealth(hp);
        }
    }
}

