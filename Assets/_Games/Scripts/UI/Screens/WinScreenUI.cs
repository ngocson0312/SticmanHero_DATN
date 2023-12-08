using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using mygame.sdk;
namespace SuperFight
{
    public class WinScreenUI : ScreenUI
    {
        [SerializeField] Button nextButton;
        [SerializeField] Button homeButton;
        [SerializeField] Button replayButton;
        [SerializeField] Button upgradeButton;
        [SerializeField] Button watchAds;
        [SerializeField] Image bgImg;
        [SerializeField] Text coinRewardText;
        [SerializeField] Text diamondRewardText;
        [SerializeField] Text levelText;
        [SerializeField] RectTransform rewardPanel;
        [SerializeField] RectTransform[] connerPanels;
        private int coinReward;
        private int diamondReward;
        private bool watchAd;
        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);
            nextButton.onClick.AddListener(Continue);
            homeButton.onClick.AddListener(BackToHome);
            replayButton.onClick.AddListener(Replay);
            upgradeButton.onClick.AddListener(OnUpgrade);
            watchAds.onClick.AddListener(ClaimX2);
        }
        public override void Active()
        {
            base.Active();
            watchAd = false;
            AudioManager.Instance.PlayOneShot("eff_win_ui", 1f);
            watchAds.gameObject.SetActive(true);
            coinReward = 100 + (DataManager.Level * 10);
            diamondReward = 5;
            coinRewardText.text = coinReward.ToString();
            diamondRewardText.text = diamondReward.ToString();
            GameManager.Instance.UpdateTask(QuestType.CLEAR_LEVEL, 1);
            bgImg.color = new Color(0, 0, 0, 0);
            rewardPanel.anchoredPosition = new Vector2(0, Screen.height);
            rewardPanel.DOAnchorPosY(35, 1f).SetEase(Ease.OutQuint);
            bgImg.DOColor(new Color(0, 0, 0, 0.3f), 1f);
            for (int i = 0; i < connerPanels.Length; i++)
            {
                connerPanels[i].anchoredPosition = new Vector2(connerPanels[i].anchoredPosition.x, -Screen.height / 2);
                connerPanels[i].DOAnchorPosY(50, 1f).SetEase(Ease.OutQuint);
            }
            DataManager.Instance.AddCoin(coinReward, 1, "", true);
            DataManager.Instance.AddDiamond(diamondReward, 1, "");
            levelText.text = "Lv." + GameManager.LevelSelected;
            if (Tutorial.TutorialStep == 0 && DataManager.Level > 4)
            {
                DataManager.Instance.AddKey(1, 0);
                Tutorial.TutorialStep = 1;
                Tutorial.Instance.TutorialClick(homeButton, 2, null);
            }
        }


        private void Replay()
        {
            //if (!watchAd)
            //{
            //    AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            //}
            GameManager.Instance.PlayGame(GameManager.LevelSelected - 1);
        }

        private void BackToHome()
        {
            //if (!watchAd && Tutorial.TutorialStep != 1)
            //{
            //    AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            //}
            uiManager.transition.Transition(1f, GameManager.Instance.BackToMenu);
        }

        private void Continue()
        {
            //if (!watchAd)
            //{
            //    AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            //}
            GameManager.Instance.PlayGame(DataManager.Level);
        }

        public void OnUpgrade()
        {
            BackToHome();
            uiManager.transition.Transition(1f, () =>
            {
                UIManager.Instance.ShowPopup<EquipmentScreen>(null);
            });
        }

        public void ClaimX2()
        {
            AdsHelper.Instance.showGift(GameRes.GetLevel(), "x2", (x) =>
            {
                if (x == AD_State.AD_REWARD_OK)
                {
                    watchAd = true;
                    DataManager.Instance.AddCoin(coinReward, 1, "", true);
                    DataManager.Instance.AddDiamond(diamondReward, 1, "");
                    coinRewardText.text = (coinReward * 2).ToString();
                    diamondRewardText.text = (diamondReward * 2).ToString();
                    watchAds.gameObject.SetActive(false);
                }
            });
        }
    }
}
