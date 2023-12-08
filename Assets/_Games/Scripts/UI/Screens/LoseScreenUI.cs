using DG.Tweening;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class LoseScreenUI : ScreenUI
    {
        public Button homeButton;
        public Button replayButton;
        public Button upgradeButton;
        // public Button watchAds;
        [SerializeField] Image bgImg;
        [SerializeField] RectTransform rewardPanel;
        [SerializeField] RectTransform[] connerPanels;
        [SerializeField] Text coinRewardText;
        int coinReward;
        bool watchAd;
        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);
            homeButton.onClick.AddListener(BackToHome);
            replayButton.onClick.AddListener(Replay);
            upgradeButton.onClick.AddListener(OnUpgrade);
            // watchAds.onClick.AddListener(ClaimX2);

        }
        public override void Active()
        {
            base.Active();
            watchAd = false;
            bgImg.color = new Color(0, 0, 0, 0);
            rewardPanel.anchoredPosition = new Vector2(0, Screen.height);
            rewardPanel.DOAnchorPosY(35, 0.5f).SetEase(Ease.OutQuint);
            bgImg.DOColor(new Color(1, 1, 1, 1f), 0.5f);
            coinReward = DataManager.Level * 5 + 50;
            coinRewardText.text = coinReward.ToString();
            // watchAds.gameObject.SetActive(true);
            AudioManager.Instance.PlayOneShot("DeathPiano", 1f);
            for (int i = 0; i < connerPanels.Length; i++)
            {
                connerPanels[i].anchoredPosition = new Vector2(connerPanels[i].anchoredPosition.x, -Screen.height / 2);
                connerPanels[i].DOAnchorPosY(50, 0.5f).SetEase(Ease.OutQuint);
            }
            if (Tutorial.TutorialStep == 0 && DataManager.Level > 4)
            {
                Tutorial.TutorialStep = 1;
                DataManager.Instance.AddKey(1, 0);
                Tutorial.Instance.TutorialClick(homeButton, 2, null);
            }
            DataManager.Instance.AddCoin(coinReward, 1, "");

        }

        private void Replay()
        {
            if (!watchAd)
            {
                AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            }
            GameManager.Instance.PlayGame(GameManager.LevelSelected);
        }

        private void BackToHome()
        {
            CameraController.Instance.CamOnStart();
            if (!watchAd && Tutorial.TutorialStep != 1)
            {
                AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            }
            uiManager.transition.Transition(1f, GameManager.Instance.BackToMenu);
        }
        public void OnUpgrade()
        {
            BackToHome();
            CameraController.Instance.CamOnStart();
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
                    coinRewardText.text = (coinReward * 2).ToString();
                    // watchAds.gameObject.SetActive(false);
                }
            });
        }
    }
}
