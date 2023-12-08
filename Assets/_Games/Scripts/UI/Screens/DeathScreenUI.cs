using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using mygame.sdk;

namespace SuperFight
{
    public class DeathScreenUI : ScreenUI
    {
        [SerializeField] Image bgImg;
        [SerializeField] Transform text;
        [SerializeField] Button continueBtn;
        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);
            continueBtn.onClick.AddListener(Continue);
        }

        private void Continue()
        {
            AdsHelper.Instance.showFull(false, 99, false, false, "lose", false);
            GameManager.Instance.Respawn();
        }

        public override void Active()
        {
            base.Active();
            continueBtn.gameObject.SetActive(false);
            bgImg.color = Color.black * 0;
            bgImg.DOFade(1, 3f).SetEase(Ease.Linear);
            text.transform.localScale = Vector3.one * 0.5f;
            text.DOScale(Vector3.one * 1f, 3f).SetEase(Ease.Linear);
            GameManager.Instance.DelayCallBack(3f, () =>
            {
                continueBtn.gameObject.SetActive(true);
            });
        }
    }
}
