using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class ChallengePopup : PopupUI
    {
        public List<ChallengeDay> challengeDays;
        public List<Button> listBtn;
        public GameObject chestHolder;
        public Button closeBtn;
        public List<Sprite> bgBtn;
        private int idx;
        private int totalPoint;
        [SerializeField] Text missionPointText;
        [SerializeField] Image fillImg;
        [SerializeField] ChestMissionButton[] chestButtons;
        [SerializeField] private RewardMission challengeRewardMission;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);

            listBtn[0].onClick.AddListener(() => OnDayBtn(0));
            listBtn[1].onClick.AddListener(() => OnDayBtn(1));
            listBtn[2].onClick.AddListener(() => OnDayBtn(2));
            listBtn[3].onClick.AddListener(() => OnDayBtn(3));
            listBtn[4].onClick.AddListener(() => OnDayBtn(4));
            listBtn[5].onClick.AddListener(() => OnDayBtn(5));
            listBtn[6].onClick.AddListener(() => OnDayBtn(6));

            for (int i = 0; i < chestButtons.Length; i++)
            {
                chestButtons[i].Initialize();
                chestButtons[i].OnPreview += Preview;
                chestButtons[i].OnClaim += OnClaimChest;
            }
            closeBtn.onClick.AddListener(Hide);

        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            SetActiveChallengeDay();
            OnDayBtn(0);
            if (challengeDays[0].listButton == null || challengeDays[0].listButton.Count <= 0)
            {
                for (int i = 0; i < ChallengeManager.Instance.dayCount; i++)
                {
                    challengeDays[i].Initialize(i, this);
                }
            }
            SetTotal();
        }

        private void Preview(ChestMissionButton obj)
        {
            for (int i = 0; i < chestButtons.Length; i++)
            {
                if (chestButtons[i] != obj)
                {
                    chestButtons[i].SetActivepreview(false);
                }
                else
                {
                    obj.SetActivepreview(!obj.previewReward.gameObject.activeSelf);
                }
            }

        }
        public void OnDayBtn(int idx)
        {
            if (idx >= ChallengeManager.Instance.dayCount) return;
            this.idx = idx;
            SetActiveChallengeDay();
            listBtn[idx].image.sprite = bgBtn[0];
            challengeDays[idx].gameObject.SetActive(true);
            SetTotal();
        }
        public void SetActiveChallengeDay()
        {
            for (int i = 0; i < challengeDays.Count; i++)
            {
                listBtn[i].image.sprite = bgBtn[1];
                challengeDays[i].gameObject.SetActive(false);
                challengeDays[i].scrollRect.verticalNormalizedPosition = 1;
            }
        }

        public void SetTotal()
        {
            totalPoint = 0;
            for (int i = 0; i < ChallengeManager.Instance.dayCount; i++)
            {
                challengeDays[i].UpdateChallenge();
                totalPoint += challengeDays[i].total;
            }

            missionPointText.text = totalPoint.ToString();
            float normalizePoint = totalPoint / 1000f;
            float currentIndex = totalPoint / 200;
            fillImg.fillAmount = Mathf.Clamp01(normalizePoint);
            for (int i = 0; i < chestButtons.Length; i++)
            {
                List<ItemRewardMission> missions = new List<ItemRewardMission>();
                missions.Add(challengeRewardMission.itemRewards[i * 2]);
                missions.Add(challengeRewardMission.itemRewards[i * 2 + 1]);
                chestButtons[i].Refresh(i + 1 <= ChallengeManager.Instance.rewardIndex, (i + 1) * 200, totalPoint, missions);
            }
        }
        public void OnClaimChest(ChestMissionButton button)
        {
            ChallengeManager.Instance.rewardIndex++;
        }
    }
}
