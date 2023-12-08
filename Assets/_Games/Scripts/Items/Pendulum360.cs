using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class Pendulum360 : ItemObject
    {
        float rotationSpeed = 90;
        Vector3 currentEulerAngles;

        public Transform PivotPoint;
        public int damage = 30;

        private void Update()
        {
            currentEulerAngles += new Vector3(0, 0, -1) * Time.deltaTime * rotationSpeed;
            PivotPoint.localEulerAngles = currentEulerAngles;
        }
        public override void Initialize()
        {

        }


        public override void ResetObject()
        {

        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamage id = col.GetComponent<IDamage>();

            if (id != null && id.controller.characterType == CharacterType.Character)
            {
                controller = id.controller;
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = controller.originalStats.health / 4;
                damageInfo.stunTime = 0.1f;
                damageInfo.characterType = CharacterType.Mob;
                col.GetComponent<IDamage>()?.TakeDamage(damageInfo);

            }

        }


    }
}
