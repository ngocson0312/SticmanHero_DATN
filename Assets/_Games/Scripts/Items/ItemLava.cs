using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ItemLava : ItemObject
    {
        public override void Initialize()
        {

        }

        public override void ResetObject()
        {

        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null && id.controller.characterType != CharacterType.Boss)
            {
                float hp = id.controller.originalStats.health;
                hp *= 0.2f;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = (int)hp;
                damageInfo.stunTime = 0.1f;
                damageInfo.characterType = CharacterType.Boss;
                id.TakeDamage(damageInfo);
            }
        }
    }
}

