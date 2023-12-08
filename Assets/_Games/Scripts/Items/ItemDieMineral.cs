using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ItemDieMineral : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = 999999;
                damageInfo.characterType = CharacterType.Boss;
                damageInfo.damageType = DamageType.TRUEDAMAGE;
                id.controller.deathDenyCount = 0;
                id.TakeDamage(damageInfo);
            }

        }
    }
}
