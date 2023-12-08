using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;
using System;

namespace SuperFight
{
    public class InappButtonfree : InappButton
    {
        public int adsCountFree
        {
            get { return PlayerPrefs.GetInt("shop_ads_count_buy_" + typeInapp + "_free", adsCount); }
            set { PlayerPrefs.SetInt("shop_ads_count_buy_" + typeInapp + "_free", value); }
        }

        public int canReceiveFree
        {
            get { return PlayerPrefs.GetInt("shop_buy_" + typeInapp + "_free", 0); }
            set { PlayerPrefs.SetInt("shop_buy_" + typeInapp + "_free", value); }
        }


        public string timeFree
        {
            get { return PlayerPrefs.GetString("shop_ads_time_" + typeInapp + "_free", "0"); }
            set { PlayerPrefs.SetString("shop_ads_time_" + typeInapp + "_free", value); }
        }
        [SerializeField] private int adsCount;
        [SerializeField] private Text adsCountTxt;
        [SerializeField] private Text timeFreeTxt;
        private float timeUpdate;

        public void Update()
        {
            if (timeUpdate <= 0)
            {
                CheckTimeFree();
                timeUpdate = 1;
            }
            else
            {
                timeUpdate -= Time.deltaTime;
            }
        }
        public override void ClickBuy(int idx)
        {
            base.ClickBuy(idx);
            if (typeInapp == TypeInapp.DIAMOND)
            {
                if (canReceiveFree == 0)
                {
                    DataManager.Instance.AddDiamond(coinReceive, 0, "BuyFreeDiamond");
                    canReceiveFree = 1;
                    SetInfo(idx);
                }
                else
                {
                    if (adsCountFree <= 0) return;
                    int ss = AdsHelper.Instance.showGift(GameRes.LevelCommon(), "buy_ads_Diamond", (value) =>
                    {
                        if (value == AD_State.AD_REWARD_OK)
                        {
                            adsCountFree--;
                            if (adsCountFree <= 0)
                            {
                                timeFree = $"{SdkUtil.CurrentTimeMilis()}";
                                CheckTimeFree();
                            }
                            adsCountTxt.text = $"{adsCountFree}/{adsCount}";
                            DataManager.Instance.AddDiamond(coinReceive, 0, "BuyFreeDiamond");
                        }
                    });
                }
            }
            else
            {
                if (canReceiveFree == 0)
                {
                    DataManager.Instance.AddCoin(coinReceive, 0, "BuyFreeCoin");
                    canReceiveFree = 1;
                    SetInfo(idx);
                }
                else
                {
                    if (adsCountFree <= 0) return;
                    int ss = AdsHelper.Instance.showGift(GameRes.LevelCommon(), "buy_ads_Coin", (value) =>
                    {
                        if (value == AD_State.AD_REWARD_OK)
                        {
                            adsCountFree--;
                            if (adsCountFree <= 0)
                            {
                                timeFree = $"{SdkUtil.CurrentTimeMilis()}";
                                CheckTimeFree();
                            }
                            adsCountTxt.text = $"{adsCountFree}/{adsCount}";
                            DataManager.Instance.AddCoin(coinReceive, 0, "BuyFreeCoin");
                        }
                    });
                }
            }
        }
        public override void SetInfo(int idx)
        {
            base.SetInfo(idx);
            SetActiveIcon();
            if (canReceiveFree != 0)
            {
                price.gameObject.SetActive(false);
                listIconButton[2].gameObject.SetActive(true);
                adsCountTxt.text = $"{adsCountFree}/{adsCount}";
            }
        }

        public void SetTxtTimeFree(long time)
        {
            timeFreeTxt.text = $"Free in: {((int)(24 - ((float)time / 3600000))):00}h {((int)(60 - ((float)time % 3600000) / 60000)):00}m {((int)(60 - ((((float)time % 3600000) % 60000) / 1000))):00}s";
        }

        public void CheckTimeFree()
        {
            long current = SdkUtil.CurrentTimeMilis();
            if (long.Parse(timeFree) <= 0) timeFree = $"{SdkUtil.CurrentTimeMilis()}";
            long delta = Math.Abs(current - long.Parse(timeFree));
            if (delta >= 86400000 || adsCountFree > 0)
            {
                timeFreeTxt.gameObject.SetActive(false);
                if (adsCountFree <= 0)
                {
                    adsCountFree = adsCount;
                    canReceiveFree = 0;
                }
            }
            else
            {
                timeFreeTxt.gameObject.SetActive(true);
                SetTxtTimeFree(delta);
            }
        }
    }
}
