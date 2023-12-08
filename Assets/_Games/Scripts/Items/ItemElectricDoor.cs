using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ItemElectricDoor : ItemObject
    {
       // [SerializeField] Electric electric;
       // [SerializeField] int damage = 30;
        [SerializeField] float timeDuration = 2.5f;
        [SerializeField] Animation anim;
        [SerializeField] Transform TopAnchor;
        [SerializeField] Transform BotAnchor;
        [SerializeField] Transform ElectricAnchor;
        float currentTimeWait;
        bool isOpen;
        


        public override void Initialize()
        {
            currentTimeWait = timeDuration;
        }

        public override void ResetObject()
        {

        }

        private void Update()
        {
            OpenDoor();
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
                        anim.Play("ElectricDoor_Close");
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
                        anim.Play("ElectricDoor_Open");
                        currentTimeWait = timeDuration;
                    }
                }
            }
        }

        void SetupDoor()
        {

        }


    }
}

