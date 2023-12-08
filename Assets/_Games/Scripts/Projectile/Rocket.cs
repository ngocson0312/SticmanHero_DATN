using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class Rocket : Projectile
    {
        public int state;
        private float timer;
        public LayerMask layerTarget;
        private Transform target;
        public float explosionRange;
        public override void Initialize(Vector2 direction, DamageInfo damageInfo)
        {
            base.Initialize(direction, damageInfo);
            state = 0;
            timer = 0;
            Destroy(gameObject, 5f);
        }

        public override void UpdateLogic()
        {
            if (!isActive) return;
            Move();
        }
        protected void Move()
        {
            timer += Time.deltaTime;
            if (state == 0)
            {
                transform.position += speed * Time.deltaTime * (Vector3)direction;
                display.transform.rotation = Quaternion.LookRotation(direction);
                if (timer >= 1)
                {
                    state = 1;
                }
            }
            if (state == 1)
            {
                Collider2D[] colls = new Collider2D[4];
                Physics2D.OverlapCircleNonAlloc(transform.position, 100f, colls, layerTarget);
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i] != null && colls[i].GetInstanceID() != damageInfo.idSender)
                    {
                        target = colls[i].transform;
                        state = 2;
                        return;
                    }
                }
            }
            if (state == 2)
            {
                Quaternion q = Quaternion.LookRotation((target.position - transform.position).normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.2f);
                transform.position += speed * 2 * Time.deltaTime * transform.forward;
                if (timer >= 1.5f)
                {
                    state = 3;
                }
            }
            if (state == 3)
            {
                transform.position += speed * 2 * Time.deltaTime * transform.forward;
            }
        }
        protected override void OnCollision(Collider2D other)
        {
            if (!isActive) return;
            if ((layerContact & (1 << other.gameObject.layer)) == 0) return;
            if (other.GetInstanceID().Equals(damageInfo.idSender)) return;
            isActive = true;
            Explosion();
        }
        private void Explosion()
        {
            display.SetActive(false);
            impact.SetActive(true);
            AudioManager.Instance.PlayOneShot("Explosion", 0.7f);
            Collider2D[] colls = new Collider2D[4];
            Physics2D.OverlapCircleNonAlloc(transform.position, explosionRange, colls, layerTarget);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i] != null && colls[i].GetComponent<IDamage>() != null && !colls[i].GetInstanceID().Equals(damageInfo.idSender))
                {
                    colls[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                }
            }

        }
        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.DrawWireSphere(transform.position, 100);
        //     Gizmos.DrawWireSphere(transform.position, explosionRange);
        // }
    }
}
