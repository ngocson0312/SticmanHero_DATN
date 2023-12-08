using System;
using mygame.sdk;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class BestSalePack : PopupUI
    {
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI originalPriceText;
        [SerializeField] RewardInfo[] rewardInfos;
        [SerializeField] Button buyBtn;
        [SerializeField] Button closeBtn;
        public static int IsOwned
        {
            get { return PlayerPrefs.GetInt("bestsale_pack_owned", 0); }
            private set { PlayerPrefs.SetInt("bestsale_pack_owned", value); }
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
            priceText.text = InappHelper.Instance.getPrice("bestsalepack");
            originalPriceText.text = InappHelper.Instance.getPrice("bestsalepackoriginal");
        }
        private void BuyPack()
        {
            InappHelper.Instance.BuyPackage("bestsalepack", "", (cb) =>
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
