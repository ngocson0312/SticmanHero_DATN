using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using mygame.sdk;
namespace SuperFight
{
    public class StarterPack : PopupUI
    {
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI originalPriceText;
        [SerializeField] RewardInfo[] rewardInfos;
        [SerializeField] Button buyBtn;
        [SerializeField] Button closeBtn;
        public static int IsOwned
        {
            get { return PlayerPrefs.GetInt("starter_pack_owned", 0); }
            private set { PlayerPrefs.SetInt("starter_pack_owned", value); }
        }
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            buyBtn.onClick.AddListener(BuyPack);
            closeBtn.onClick.AddListener(Hide);
        }
        public override void Show(Action onClose)
        {
            base.Show(onClose);
            priceText.text = InappHelper.Instance.getPrice("starterpack");
            originalPriceText.text = InappHelper.Instance.getPrice("starterpackoriginal");
        }
        private void BuyPack()
        {
            InappHelper.Instance.BuyPackage("starterpack", "", (cb) =>
            {
                if (cb.status == 1)
                {
                    IsOwned = 1;
                    RewardPopup rewardPopup = uiManager.ShowPopup<RewardPopup>(null);
                    rewardPopup.ShowReward(rewardInfos);
                    Hide();
                }
            });
        }
    }
}
