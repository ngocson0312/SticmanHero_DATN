using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;

namespace SuperFight
{
    public class MainTab : TabPanel
    {
        [Header("Button")]
        public Button luckyWheel;
        public Button missionBtn;
        public Button dailyBtn;
        //public Button removeAdsBtn;
       // public Button challenge;
        public void Initialize()
        {
            luckyWheel.onClick.AddListener(OnLucKyWheel);
            //removeAdsBtn.onClick.AddListener(OnRemoveAds);
            dailyBtn.onClick.AddListener(DailyReward);
            missionBtn.onClick.AddListener(OnMission);
          //  challenge.onClick.AddListener(OnChallengeDay);
        }

        public void OnMission()
        {
            UIManager.Instance.ShowPopup<QuestUI>(null);
        }

        //public void OnRemoveAds()
        //{
        //    InappHelper.Instance.BuyPackage("removeads", "RemoveAds", null);
        //}

        //public void OnChallengeDay()
        //{
        //    UIManager.Instance.ShowPopup<ChallengePopup>(null);
        //}
        public void OnLucKyWheel()
        {
            UIManager.Instance.ShowPopup<LuckWheelPopup>(null);
        }

        public void DailyReward()
        {
            UIManager.Instance.ShowPopup<DailyReward>(null);
        }
        public override void Active()
        {
            base.Active();
            
        }
    }
}
