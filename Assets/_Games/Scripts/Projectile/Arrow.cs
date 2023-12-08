using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Arrow : Projectile
    {
        private int state;
        private float timer;
        public override void Initialize(Vector2 direction, DamageInfo damageInfo)
        {
            base.Initialize(direction, damageInfo);
            display.transform.rotation = Quaternion.LookRotation(direction);
            Deactive(2);
            state = 0;
            timer = 0;
        }
        public override void UpdateLogic()
        {
            if (!isActive) return;
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                direction.y -= Time.deltaTime;
                if (direction.y < -1)
                {
                    direction.y = -1;
                }
                display.transform.rotation = Quaternion.LookRotation(direction.normalized);
            }
            base.UpdateLogic();
        }
        protected override void OnCollision(Collider2D other)
        {
            if (!isActive) return;
            if ((layerContact & (1 << other.gameObject.layer)) == 0) return;
            if (other.GetInstanceID().Equals(damageInfo.idSender)) return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isActive = false;
                damageCollider.enabled = false;
            }
            else
            {
                IDamage ida = other.GetComponent<IDamage>();
                if (ida != null)
                {
                    List<IDamage> listContact = new List<IDamage>();
                    listContact.Add(ida);
                    if (!ida.controller.isInvincible)
                    {
                        damageCollider.enabled = false;
                        display.SetActive(false);
                        OnContactCollision(listContact);
                        Deactive(0);
                    }
                }
            }
        }
    }
}

