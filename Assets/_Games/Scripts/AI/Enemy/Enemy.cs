using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public abstract class Enemy : Controller
    {
        public float speed = 5f;
        private State currentState;
        public SpriteBar healthBar;
        public ParticleSystem hitFX;
        protected Collider2D selfCollider;
        public ParticleSystem CiticalVfx;
        public ParticleSystem buffFX;
        public Weapon weapon;
        public bool isUnstopable;
        public int coinReward;
        public virtual void Initialize()
        {
            selfCollider = GetComponent<Collider2D>();
            core.Initialize(this);
            animatorHandle.Initialize(this);
            weapon.Initialize(this);
            weapon.OnEquip();
            isActive = false;
            GameManager.OnPause += Pause;
            GameManager.OnResume += Resume;
            animatorHandle.OnEventAnimation += OnEventAnim;
        }

        private void OnEventAnim(string obj)
        {
            if (obj.Equals("ActiveUnstopable"))
            {
                isUnstopable = true;
            }
            if (obj.Equals("DeactiveUnstopable"))
            {
                isUnstopable = false;
            }
        }

        public void ConfigStats(CharacterStats characterStats, int coinReward)
        {
            originalStats = new CharacterStats(characterStats);
            runtimeStats = new CharacterStats(originalStats);
            this.coinReward = coinReward;
        }
        public void Active()
        {
            gameObject.SetActive(true);
            isActive = true;
            selfCollider.enabled = true;
            ResetController();
        }
        protected virtual void LogicUpdate()
        {
            if (GameManager.GameState != GameState.PLAYING) return;
            isInteracting = animatorHandle.GetBool("IsInteracting");
            core.UpdateLogicCore();
            if (currentState != null)
            {
                currentState.UpdateLogic();
            }
            weapon.OnUpdate();
        }
        public override void SetHealth(int amount)
        {
            base.SetHealth(amount);
            healthBar.UpdateBar(NormalizeHealth());
            buffFX.Play();
        }
        protected virtual void PhysicUpdate()
        {
            if (GameManager.GameState != GameState.PLAYING) return;
            if (currentState != null)
            {
                currentState.UpdatePhysic();
            }
        }
        private void Update()
        {
            if (!isActive) return;
            LogicUpdate();
            UpdateStatusEffects();
        }
        private void FixedUpdate()
        {
            if (!isActive) return;
            PhysicUpdate();
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
        public void StopStateMachine()
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }
        }
        public abstract Controller GetTargetInView();
        public abstract Controller GetTargetInView(Bounds bounds);
        public override void Die(bool deactiveCharacter)
        {
            if (!isActive) return;
            base.Die(deactiveCharacter);
            SpawnCoin();
            healthBar.Deactive();
            core.Deactive();
            selfCollider.enabled = false;
            GameManager.Instance.UpdateTask(QuestType.KILL_ENEMY, 1);
        }
        private void SpawnCoin()
        {
            coinReward = 20 + (GameManager.LevelSelected * 3);
            int count = Random.Range(3, 5);
            for (int i = 0; i < count; i++)
            {
                var coin = FactoryObject.Spawn<ItemCoin>("Item", "Coin");
                coin.transform.position = transform.position + Vector3.up;
                coin.Initialize(coinReward / count);
            }
        }
        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            // for (int i = 0; i < statusEffects.Count; i++)
            // {
            //     if (statusEffects[i].statusName == StatusEffectType.BURNING)
            //     {
            //         int bonus = (int)(damageInfo.damage * 15f / 100f);
            //         damageInfo.damage += bonus;
            //     }
            //     // if (statusEffects[i].statusName == StatusEffectType.ELECTROCUTE && damageInfo.isCritical)
            //     // {

            //     //   //  int bonus = (int)(damageInfo.damage / 2);
            //     //    // damageInfo.damage += bonus;

            //     // }
            //     //if(statusEffects[i].statusName == StatusEffectType.CURSE)
            //     //{
            //     //    int bonus = (int)(runtimeStats.health);
            //     //    damageInfo.damage += bonus;
            //     //}
            // }
            if (damageInfo.listEffect != null)
            {
                for (int i = 0; i < damageInfo.listEffect.Count; i++)
                {
                    AddStatusEffect(damageInfo.listEffect[i]);
                }
            }
            runtimeStats.health -= damageInfo.damage;
            if (damageInfo.impactSound)
            {
                AudioManager.Instance.PlayOneShot(damageInfo.impactSound, 1f);
            }
            else
            {
                AudioManager.Instance.PlayOneShot("eff_be_hit" + Random.Range(1, 4), 1f);
            }
            base.OnTakeDamage(damageInfo);
        }
        private void OnDestroy()
        {
            GameManager.OnPause -= Pause;
            GameManager.OnResume -= Resume;
        }
    }
}

