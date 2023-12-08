using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Spike : MonoBehaviour
    {
        Controller controller;

        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null)
            {
                controller = id.controller;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = controller.originalStats.health / 5;
                damageInfo.stunTime = 0.1f;
                damageInfo.characterType = CharacterType.Boss;
                id.TakeDamage(damageInfo);
            }

        }
    }
}

