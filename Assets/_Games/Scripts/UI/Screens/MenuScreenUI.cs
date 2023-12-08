using System;
using System.Collections;
using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class MenuScreenUI : ScreenUI
    {
        [Header("Button")]
        public Button settingBtn;
        public Button shopBtn;
        public Button upgradeBtn;
        public Button upgradePlayerBtn;
        public Button luckyWheel;
        public Button missionBtn;
        public Button dailyBtn;
        //public Button removeAdsBtn;
       // public Button challengeBtn;
        public Button playBtn;
        public Text levelText;
        public Button selectLevelBtn;
        public LevelPanel levelPanel;
        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);
            levelPanel.Initialize();
            playBtn.onClick.AddListener(Play);
            selectLevelBtn.onClick.AddListener(OpenSelectPanel);
            upgradeBtn.onClick.AddListener(OnUpgrade);
            shopBtn.onClick.AddListener(OnShop);
            settingBtn.onClick.AddListener(OnSetting);
            luckyWheel.onClick.AddListener(OnLucKyWheel);
            //removeAdsBtn.onClick.AddListener(OnRemoveAds);
            dailyBtn.onClick.AddListener(DailyReward);
            missionBtn.onClick.AddListener(OnMission);
           // challengeBtn.onClick.AddListener(OnChallengeDay);
            upgradePlayerBtn.onClick.AddListener(UpgradePlayer);
            QuestManager.OnTaskUpdate += OnTaskUpdate;
        }

        private void UpgradePlayer()
        {
            var x = uiManager.ShowPopup<EquipmentScreen>(null);
            x.EnablePanel(1);
        }

        private void OnTaskUpdate(int[] type, bool obj)
        {
            missionBtn.transform.Find("Notify").gameObject.SetActive(obj);
        }

     
        private void OpenSelectPanel()
        {
            levelPanel.Show();
        }

        public override void Active()
        {
            base.Active();
            CameraController.Instance.SetOffset(Vector3.up);
            AudioManager.Instance.StopAllMusic();
            AudioManager.Instance.PlayMusic("music_menu", 0.6f, true);
            levelText.text = "Level " + DataManager.Level;
            if (Tutorial.TutorialStep == 1)
            {
                Tutorial.TutorialStep = 2;
                Tutorial.Instance.TutorialClick(shopBtn, 0.5f, null);
            }
            
        }
        public void OnSetting()
        {
            var x = uiManager.ShowPopup<SettingPopup>(null);
            x.ShowType(false);
        }

        public void OnShop()
        {
            uiManager.ShowPopup<ShopScreenUI>(() =>
            {
                if (Tutorial.TutorialStep == 3)
                {
                    Tutorial.TutorialStep = 4;
                    Tutorial.Instance.TutorialClick(upgradeBtn, 0.5f, null);
                }
            });
        }

        public void OnUpgrade()
        {
            uiManager.ShowPopup<EquipmentScreen>(null);
        }
        private void Play()
        {
            GameManager.Instance.PlayGame(DataManager.Level);
        }
        public void OnMission()
        {
            uiManager.ShowPopup<QuestUI>(null);
        }

        public void OnRemoveAds()
        {
            InappHelper.Instance.BuyPackage("removeads", "RemoveAds", null);
        }

        public void OnChallengeDay()
        {
            uiManager.ShowPopup<ChallengePopup>(null);
        }
        public void OnLucKyWheel()
        {
            uiManager.ShowPopup<LuckWheelPopup>(null);
        }

        public void DailyReward()
        {
            uiManager.ShowPopup<DailyReward>(null);
        }
    }
}
