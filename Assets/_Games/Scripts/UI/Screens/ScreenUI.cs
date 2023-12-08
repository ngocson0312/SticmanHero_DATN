using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public abstract class ScreenUI : MonoBehaviour
    {
        protected UIManager uiManager;
        public virtual void Initialize(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }
        public virtual void Active()
        {
            gameObject.SetActive(true);
        }
        public virtual void Deactive()
        {
            gameObject.SetActive(false);
        }
    }
}
