using System;
using UnityEngine;
namespace SuperFight
{
    public abstract class PopupUI : MonoBehaviour
    {
        protected UIManager uiManager;
        protected Action onClose;
        public virtual void Initialize(UIManager manager)
        {
            this.uiManager = manager;
        }
        public virtual void Show(Action onClose)
        {
            this.onClose = onClose;
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            onClose?.Invoke();
            gameObject.SetActive(false);
            onClose = null;
        }
    }
}

