using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ItemMineralogy : MonoBehaviour
    {
        Controller controller;
        int dam;
        private float timer;

        private bool isLava;
        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();
            if (id != null && id.controller.characterType == CharacterType.Character)
            {
                controller = id.controller;
                isLava = true;
                dam = id.controller.originalStats.health / 6;

            }

        }

        private void OnTriggerExit2D(Collider2D col)
        {
            isLava = false;
        }

        private void Update()
        {
            if (!controller) return;
            timer -= Time.deltaTime;
            if (timer <= 0 && isLava)
            {

                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = dam;
                damageInfo.stunTime = 0.1f;

                controller.OnTakeDamage(damageInfo);
                timer = 1f;

            }

        }




    }
}
