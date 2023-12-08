using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class FrostbiteEffect : StatusEffect
    {
        [SerializeField] float frozenTime;
        [SerializeField] ParticleSystem frozenEff;
        [SerializeField] ParticleSystem frozenBrokenEff;
        public bool isFreerzing;
        public int frozenPoint;
        private int amorbackup;
        public override void Initialize(StatusEffectData statusEffectData)
        {
            base.Initialize(statusEffectData);
            statusName = StatusEffectType.FROSTBITE;
            frozenEff.Stop();
            frozenBrokenEff.Stop();
        }
        public override void OnStartEffect(Controller controller)
        {
            base.OnStartEffect(controller);
            amorbackup = controller.runtimeStats.armor;
            controller.SetControllerSpeed(.7f);
        }
        public override void OnFinishEffect()
        {
            controller.runtimeStats.armor = amorbackup;
            controller.SetControllerSpeed(1f);
            frozenEff.Stop();
            frozenBrokenEff.Play();
            OnComplete?.Invoke();
            Destroy(gameObject, 1f);
        }
        public void Freeze()
        {
            frozenTime = 3f;
            isFreerzing = true;
            controller.SetControllerSpeed(0);
            controller.core.movement.SetVelocityZero();
            frozenEff.Play();
        }
        public override void UpdateEffect()
        {
            frozenTime -= Time.deltaTime;
            if (isFreerzing && frozenTime <= 0)
            {
                OnFinishEffect();
            }
        }
    }
}

