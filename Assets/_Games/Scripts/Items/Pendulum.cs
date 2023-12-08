using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SuperFight
{
    public class Pendulum : ItemObject
    {


        public float rotateAngle;
        public float rotateDuration;
        public float timeDelay;

        public override void Initialize()
        {
            this.transform.eulerAngles = new Vector3(0, 0, -rotateAngle);
            this.transform.DORotate(new Vector3(0, 0, rotateAngle), rotateDuration).SetDelay(timeDelay).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }

        public override void ResetObject()
        {

        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null && id.controller.characterType == CharacterType.Character)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = id.controller.originalStats.health / 4;
                damageInfo.stunTime = 0.1f;
                damageInfo.characterType = CharacterType.Mob;
                col.GetComponent<IDamage>()?.TakeDamage(damageInfo);

            }

        }

    }
}
