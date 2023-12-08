using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class FireHoseItem : ItemObject
    {
        //  [SerializeField] int damage = 50;
        [SerializeField] float timeDuration = 2.5f;
        public Animation ani;
        public GameObject Fire;

        [SerializeField] ParticleSystem fireVfx;

        float currentTimeWait;
        bool isOpen;

        public override void Initialize()
        {
            currentTimeWait = timeDuration;
            //  electric.SetDamage(damage);
        }

        public override void ResetObject()
        {

        }

        private void Update()
        {
            OpenDoor();
            if (Fire.activeInHierarchy == true)
            {

                OnableVfx();
            }
            else
            {
                DisableVfx();
            }
        }



        public void OnableVfx()
        {
            fireVfx.Play();
        }

        public void DisableVfx()
        {
            fireVfx.Stop();
        }
        void OpenDoor()
        {
            if (isOpen)
            {
                if (currentTimeWait > 0)
                {
                    currentTimeWait -= Time.fixedDeltaTime;
                    if (currentTimeWait <= 0)
                    {
                        isOpen = false;
                        ani.Play("FireDoorClose");

                        currentTimeWait = timeDuration;
                    }
                }
            }
            else
            {
                if (currentTimeWait > 0)
                {
                    currentTimeWait -= Time.fixedDeltaTime;
                    if (currentTimeWait <= 0)
                    {
                        isOpen = true;
                        ani.Play("FireDoorOpen");

                        currentTimeWait = timeDuration;
                    }
                }
            }
        }


    }
}
