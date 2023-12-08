using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ItemSpikeFallTrap : ItemObject
    {
        [SerializeField] GameObject SprSpike;
        [SerializeField] ParticleSystem FXBreak;
        [SerializeField] float distance;
        [SerializeField] LayerMask layerMask;
        [SerializeField] Transform EndAnchor;
        [SerializeField] Vector2 OriginalPos;
        bool isBreak = false;
        bool isActive = false;

        public bool isLevelBoss = false;

        public override void Initialize()
        {
            distance = transform.position.y - EndAnchor.position.y;
            if (distance <= 7.5)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1;
            }
            isActive = false;
            OriginalPos = transform.position;
        }

        public override void ResetObject()
        {
            SprSpike.SetActive(true);
            transform.position = OriginalPos;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            isActive = false;
            isBreak = false;
        }
        private void Update()
        {
            if (isLevelBoss) return;
            if (!isBreak)
            {
                RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down, distance, layerMask);
                if (raycast.collider != null)
                {
                    if (raycast.collider.gameObject.layer == LayerMask.NameToLayer(Constant.layerMainPlayer))
                    {
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                        isActive = true;
                    }
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!isActive) return;
            IDamage id = col.GetComponent<IDamage>();
            if (id != null && id.controller.characterType == CharacterType.Character)
            {
                controller = id.controller;
                SpikeBreak();
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = (int)(controller.originalStats.health * 0.1f);
                damageInfo.stunTime = 0.1f;
                col.GetComponent<IDamage>()?.TakeDamage(damageInfo);
            }
        }

        public void Active()
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            isActive = true;
        }

        void SpikeBreak()
        {
            isBreak = true;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            FXBreak.Play();
            SprSpike.SetActive(false);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            distance = transform.position.y - EndAnchor.position.y;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * Mathf.Abs(distance));
        }


    }
}

