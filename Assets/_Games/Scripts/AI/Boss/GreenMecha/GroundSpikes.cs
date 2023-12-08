using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SuperFight
{
    public class GroundSpikes : MonoBehaviour
    {
        [SerializeField] SpikeCollider[] spikeCollider;
        public List<Collider2D> listContact;
        public void Active(DamageInfo damageInfo)
        {
            listContact = new List<Collider2D>();
            GetComponent<ParticleSystem>().Play();
            StartCoroutine(ActiveSpikes(damageInfo));
        }
        IEnumerator ActiveSpikes(DamageInfo damageInfo)
        {
            for (int i = 0; i < spikeCollider.Length; i++)
            {
                spikeCollider[i].Active(damageInfo, this);
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < spikeCollider.Length; i++)
            {
                spikeCollider[i].Deactive();
            }
        }
    }
}

