using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class SpikeCollider : MonoBehaviour
    {
        DamageInfo damageInfo;
        public LayerMask layerContact;
        private Collider2D coll;
        private GroundSpikes groundSpikes;
        public AudioClip activeSound;
        public void Active(DamageInfo damageInfo, GroundSpikes groundSpikes)
        {
            gameObject.SetActive(true);
            AudioManager.Instance.PlayOneShot(activeSound, 0.5f);
            if (coll == null)
            {
                coll = GetComponent<Collider2D>();
            }
            this.groundSpikes = groundSpikes;
            this.damageInfo = damageInfo;
            this.damageInfo.stunForce = new Vector2(15f, 15f);
            this.damageInfo.stunTime = 0.6f;
        }
        private void FixedUpdate()
        {
            Collider2D[] targets = Physics2D.OverlapBoxAll(coll.bounds.center, coll.bounds.size, 0, layerContact);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].GetInstanceID() != damageInfo.idSender && targets[i].GetComponent<IDamage>() != null && !groundSpikes.listContact.Contains(targets[i]))
                {
                    targets[i].GetComponent<IDamage>().TakeDamage(damageInfo);
                    groundSpikes.listContact.Add(targets[i]);
                }
            }
        }
        public void Deactive()
        {
            gameObject.SetActive(false);
        }
    }
}

