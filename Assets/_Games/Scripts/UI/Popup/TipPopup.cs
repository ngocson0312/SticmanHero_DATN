using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class TipPopup : PopupUI
    {
        [SerializeField] Button closeBtn;
        public override void Show(Action onClose)
        {
            base.Show(onClose);
            closeBtn.onClick.AddListener(Hide);
            closeBtn.gameObject.SetActive(false);
            GameManager.Instance.DelayCallBack(2, () =>
            {
                closeBtn.gameObject.SetActive(true);
            });
        }
    }
}
