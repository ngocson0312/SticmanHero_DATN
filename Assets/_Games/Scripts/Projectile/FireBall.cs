using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    [RequireComponent(typeof(Collider2D))]
    public class FireBall : Projectile
    {
        public float radius = 1f;
        public Vector3 offset;
        public float deactiveTime = 5;
        public override void Initialize(Vector2 direction, DamageInfo damageInfo)
        {
            base.Initialize(direction, damageInfo);
            Deactive(deactiveTime);
        }
        protected override void OnCollision(Collider2D other)
        {
            if (!isActive) return;
            if ((layerContact & (1 << other.gameObject.layer)) == 0) return;
            if (other.GetInstanceID().Equals(damageInfo.idSender)) return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isActive = false;
                display.SetActive(false);
                impact.SetActive(true);
                GetComponent<Collider2D>().enabled = false;
                OnExplode();
            }
            else
            {
                IDamage id = other.GetComponent<IDamage>();
                if (id != null && !id.controller.isInvincible && id.controller.characterType != damageInfo.characterType)
                {
                    OnExplode();
                }
            }
        }

        private void OnExplode()
        {
            var colls = new Collider2D[5];
            Physics2D.OverlapCircleNonAlloc(transform.position + offset, radius, colls, layerContact);
            bool deactive = false;
            List<IDamage> listContact = new List<IDamage>();
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i] == null) continue;
                IDamage id = colls[i].GetComponent<IDamage>();
                if (id != null && !id.controller.isInvincible)
                {
                    deactive = true;
                    listContact.Add(id);
                }
            }
            OnContactCollision(listContact);
            if (deactive)
            {
                isActive = false;
                display.SetActive(false);
                impact.SetActive(true);
                GetComponent<Collider2D>().enabled = false;
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
    }
}

