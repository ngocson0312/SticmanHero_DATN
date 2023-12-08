using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using mygame.sdk;
using Spine.Unity;

namespace SuperFight
{
    public class LuckWheelPopup : PopupUI
    {
        public int adsCountDay
        {
            get { return PlayerPrefs.GetInt("LuckWheel_ads_count", 3); }
            set { PlayerPrefs.SetInt("LuckWheel_ads_count", value); }
        }

        public string timeFree
        {
            get { return PlayerPrefs.GetString("time_luckWheel_spin_free", "0"); }
            set { PlayerPrefs.SetString("time_luckWheel_spin_free", value); }
        }
        [Header("Buttons")]
        [SerializeField] Button buttonSpin;
        [SerializeField] Button buttonAds;
        [SerializeField] Button buttonBack;
        [Header("Components")]
        public Text priceTxt;
        public Text adsCountTxt;
        public Spin spin;
        public int idBox;
        public int spinCount;
        public RewardSpin rewardSpin;
        public GameObject timeTxtObj;
        public Text timeFreeTxt;
        public int adsCount;
        private float timeUpdate;
        [SerializeField] private int price;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(uiManager);
            spin.Initialize(this);
            rewardSpin.Initialize();
            buttonSpin.onClick.AddListener(ClickButtonSpin);
            buttonAds.onClick.AddListener(ClickWatchAds);
            buttonBack.onClick.AddListener(Hide);
            priceTxt.text = $"{price}";
            adsCountTxt.text = $"{adsCountDay}/3";
            CheckButton();
        }

        public void Update()
        {
            if (timeUpdate <= 0)
            {
                CheckTimeFreeChest();
                timeUpdate = 1;
            }
            else
            {
                timeUpdate -= Time.deltaTime;
            }
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            CheckTimeFreeChest();
        }
        void ClickWatchAds()
        {
            if (spin._isSpinning) return;
            int ss = AdsHelper.Instance.showGift(GameRes.LevelCommon(), "buy_ads_spin", (value) =>
            {
                if (value == AD_State.AD_REWARD_OK)
                {
                    spin.OnSpin();
                    adsCountDay--;
                    adsCountTxt.text = $"{adsCountDay}/{adsCount}";
                    if (adsCountDay <= 0)
                    {
                        timeFree = $"{SdkUtil.CurrentTimeMilis()}";
                        CheckTimeFreeChest();
                    }
                    CheckButton();
                }
            });
        }
        void ClickButtonSpin()
        {
            if (spin._isSpinning) return;
            if (DataManager.Diamond >= price)
            {
                DataManager.Instance.AddDiamond(-price, 0, "BuySpin");
                GameManager.Instance.UpdateTask(QuestType.SPEND_DIAMOND, price);
                spin.OnSpin();
            }
            else
            {
                UIManager.Instance.GetPopup<NotificePopup>().NotEnoughDiamond(this.gameObject);
            }
        }

        public void CheckButton()
        {
            if (adsCountDay > 0)
            {
                SetActiveButton(true);

            }
            else
            {
                SetActiveButton(false);
            }
        }
        public void SetActiveButton(bool status)
        {
            buttonAds.gameObject.SetActive(status);
            timeTxtObj.SetActive(!status);
            buttonSpin.gameObject.SetActive(!status);
        }

        public void CheckTimeFreeChest()
        {
            long current = SdkUtil.CurrentTimeMilis();
            if (long.Parse(timeFree) <= 0) timeFree = $"{SdkUtil.CurrentTimeMilis()}";
            long delta = Math.Abs(current - long.Parse(timeFree));
            if (delta >= 86400000 || adsCountDay > 0)
            {
                SetActiveButton(true);
                if (adsCountDay <= 0)
                {
                    adsCountDay = adsCount;
                }
            }
            else
            {
                SetActiveButton(false);
                SetTxtTimeFree(delta);
            }
        }

        public void SetTxtTimeFree(long time)
        {
            timeFreeTxt.text = $"{((int)(24 - ((float)time / 3600000))):00}:{((int)(60 - ((float)time % 3600000) / 60000)):00}:{((int)(60 - ((((float)time % 3600000) % 60000) / 1000))):00}";
        }
    }
}

