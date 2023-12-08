using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace SuperFight
{
    public class QuestUI : PopupUI
    {
        [SerializeField] QuestTaskButton buttonPrefab;
        [SerializeField] ScrollRect scrollDaily;
        [SerializeField] ScrollRect scrollAchievements;
        [SerializeField] Button closeButton;
        [SerializeField] Button dailyButton;
        [SerializeField] Button weeklyButton;
        [SerializeField] Button achievementsButton;
        [SerializeField] GameObject[] notifyObjs;
        [SerializeField] Text missionPointText;
        [SerializeField] RectTransform popup;
        [SerializeField] GameObject chestHolder;
        [SerializeField] Image bgImg;
        [SerializeField] Image fillImg;
        [SerializeField] Sprite[] iconBg;
        [SerializeField] ChestMissionButton[] chestButtons;
        private List<QuestTaskButton> questTaskButtons;
        private DailyQuest dailyQuest;
        private WeeklyQuest weeklyQuest;
        private AchievementsQuest achivementQuest;
        [SerializeField] private RewardMission dayRewardMission;
        [SerializeField] private RewardMission weekRewardMission;
        private QuestCategory questCategory;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            closeButton.onClick.AddListener(Hide);
            dailyButton.onClick.AddListener(OnDaily);
            weeklyButton.onClick.AddListener(OnWeekly);
            achievementsButton.onClick.AddListener(OnAchivement);

            questTaskButtons = new List<QuestTaskButton>();
            for (int i = 0; i < chestButtons.Length; i++)
            {
                chestButtons[i].Initialize();
                chestButtons[i].OnPreview += Preview;
                chestButtons[i].OnClaim += OnClaimChest;
            }
            QuestManager.OnTaskUpdate += OnTaskUpdate;
        }

        private void OnTaskUpdate(int[] types, bool complete)
        {
            for (int i = 0; i < types.Length; i++)
            {
                notifyObjs[i].SetActive(types[i] != 0);
            }
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

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            bgImg.color = new Color(0, 0, 0, 0);
            bgImg.DOFade(0.5f, 0.5f);
            popup.anchoredPosition = new Vector2(0, Screen.height);
            popup.DOAnchorPosY(-80f, 0.5f).SetEase(Ease.OutQuart);
            OnDaily();
        }
        public override void Hide()
        {
            onClose?.Invoke();
            bgImg.color = new Color(0, 0, 0, 0);
            popup.DOAnchorPosY(Screen.height, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            onClose = null;
        }

        private void OnDaily()
        {
            HideObj();
            chestHolder.SetActive(true);
            scrollDaily.gameObject.SetActive(true);
            dailyButton.image.sprite = iconBg[1];
            questCategory = QuestCategory.DAILY;
            if (dailyQuest == null)
            {
                dailyQuest = QuestManager.Instance.dailyQuestObj;
            }
            int totalPoint = 0;
            if (questTaskButtons.Count > dailyQuest.listQuest.Count)
            {
                for (int i = 0; i < questTaskButtons.Count; i++)
                {
                    if (i < dailyQuest.listQuest.Count)
                    {
                        questTaskButtons[i].UpdateTask(dailyQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollDaily.content);
                        if (dailyQuest.listQuest[i].claimed)
                        {
                            totalPoint += dailyQuest.listQuest[i].pointMission;
                        }
                    }
                    else
                    {
                        questTaskButtons[i].UpdateTask(null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dailyQuest.listQuest.Count; i++)
                {
                    if (i < questTaskButtons.Count)
                    {
                        questTaskButtons[i].UpdateTask(dailyQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollDaily.content);
                    }
                    else
                    {
                        QuestTaskButton questTask = Instantiate(buttonPrefab, scrollDaily.content);
                        questTask.Initialize();
                        questTask.OnClaim += OnClaimMission;
                        questTask.UpdateTask(dailyQuest.listQuest[i]);
                        questTask.SetParent(scrollDaily.content);
                        questTaskButtons.Add(questTask);
                    }
                    if (dailyQuest.listQuest[i].claimed)
                    {
                        totalPoint += dailyQuest.listQuest[i].pointMission;
                    }
                }
            }
            missionPointText.text = totalPoint.ToString();
            float normalizePoint = totalPoint / 100f;
            float currentIndex = totalPoint / 20;
            fillImg.fillAmount = Mathf.Clamp01(normalizePoint);
            for (int i = 0; i < chestButtons.Length; i++)
            {
                List<ItemRewardMission> missions = new List<ItemRewardMission>();
                missions.Add(dayRewardMission.itemRewards[i * 2]);
                missions.Add(dayRewardMission.itemRewards[i * 2 + 1]);
                chestButtons[i].Refresh(i + 1 <= dailyQuest.rewardIndex, (i + 1) * 20, totalPoint, missions);
            }
            UpdateComplete(questTaskButtons, scrollDaily);

        }

        private void OnWeekly()
        {
            HideObj();
            chestHolder.SetActive(true);
            scrollDaily.gameObject.SetActive(true);
            weeklyButton.image.sprite = iconBg[1];
            questCategory = QuestCategory.WEEKLY;
            if (weeklyQuest == null)
            {
                weeklyQuest = QuestManager.Instance.weeklyQuestObj;
            }
            int totalPoint = 0;
            if (questTaskButtons.Count > weeklyQuest.listQuest.Count)
            {
                for (int i = 0; i < questTaskButtons.Count; i++)
                {
                    if (i < weeklyQuest.listQuest.Count)
                    {
                        questTaskButtons[i].UpdateTask(weeklyQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollDaily.content);
                        if (weeklyQuest.listQuest[i].claimed)
                        {
                            totalPoint += weeklyQuest.listQuest[i].pointMission;
                        }
                    }
                    else
                    {
                        questTaskButtons[i].UpdateTask(null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < weeklyQuest.listQuest.Count; i++)
                {
                    if (i < questTaskButtons.Count)
                    {
                        questTaskButtons[i].UpdateTask(weeklyQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollDaily.content);
                    }
                    else
                    {
                        QuestTaskButton questTask = Instantiate(buttonPrefab, scrollDaily.content);
                        questTask.Initialize();
                        questTask.OnClaim += OnClaimMission;
                        questTask.UpdateTask(weeklyQuest.listQuest[i]);
                        questTask.SetParent(scrollDaily.content);
                        questTaskButtons.Add(questTask);
                    }
                    if (weeklyQuest.listQuest[i].claimed)
                    {
                        totalPoint += weeklyQuest.listQuest[i].pointMission;
                    }
                }
            }
            missionPointText.text = totalPoint.ToString();
            float normalizePoint = totalPoint / 100f;
            float currentIndex = totalPoint / 20;
            fillImg.fillAmount = Mathf.Clamp01(normalizePoint);
            for (int i = 0; i < chestButtons.Length; i++)
            {
                List<ItemRewardMission> missions = new List<ItemRewardMission>();
                missions.Add(weekRewardMission.itemRewards[i * 2]);
                missions.Add(weekRewardMission.itemRewards[i * 2 + 1]);
                chestButtons[i].Refresh(i + 1 <= weeklyQuest.rewardIndex, (i + 1) * 20, totalPoint, missions);
            }
            UpdateComplete(questTaskButtons, scrollDaily);
        }

        private void OnAchivement()
        {
            HideObj();
            chestHolder.SetActive(false);
            scrollAchievements.gameObject.SetActive(true);
            achievementsButton.image.sprite = iconBg[1];
            questCategory = QuestCategory.ARCHIVEMENT;
            if (achivementQuest == null)
            {
                achivementQuest = QuestManager.Instance.achievementsQuestObj;
            }
            if (questTaskButtons.Count > achivementQuest.listQuest.Count)
            {
                for (int i = 0; i < questTaskButtons.Count; i++)
                {
                    if (i < achivementQuest.listQuest.Count)
                    {
                        questTaskButtons[i].UpdateTask(achivementQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollAchievements.content);
                    }
                    else
                    {
                        questTaskButtons[i].UpdateTask(null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < achivementQuest.listQuest.Count; i++)
                {
                    if (i < questTaskButtons.Count)
                    {
                        questTaskButtons[i].UpdateTask(achivementQuest.listQuest[i]);
                        questTaskButtons[i].SetParent(scrollAchievements.content);
                    }
                    else
                    {
                        QuestTaskButton questTask = Instantiate(buttonPrefab, scrollAchievements.content);
                        questTask.Initialize();
                        questTask.OnClaim += OnClaimMission;
                        questTask.UpdateTask(achivementQuest.listQuest[i]);
                        questTask.SetParent(scrollAchievements.content);
                        questTaskButtons.Add(questTask);
                    }
                }
            }
            UpdateComplete(questTaskButtons, scrollAchievements);
        }

        public void UpdateComplete(List<QuestTaskButton> questTaskButtons, ScrollRect scrollRect)
        {
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                QuestTaskButton questTaskButton = scrollRect.content.GetChild(i).GetComponent<QuestTaskButton>();
                if (questTaskButton.questData.current >= questTaskButton.questData.require)
                {
                    scrollRect.content.GetChild(i).SetSiblingIndex(0);
                }
            }
        }
        public void OnClaimChest(ChestMissionButton chestMissionButton)
        {
            switch (questCategory)
            {
                case QuestCategory.DAILY:
                    dailyQuest.rewardIndex++;
                    //OnDaily();
                    break;
                case QuestCategory.WEEKLY:
                    weeklyQuest.rewardIndex++;
                    //OnWeekly();
                    break;
            }
        }
        private void OnClaimMission(QuestData questData)
        {
            switch (questCategory)
            {
                case QuestCategory.DAILY:
                    OnDaily();
                    GameManager.Instance.UpdateTask(QuestType.COMPLETE_DAILY_MISSIONS, 1);
                    break;
                case QuestCategory.WEEKLY:
                    OnWeekly();
                    GameManager.Instance.UpdateTask(QuestType.COMPLETE_WEEKLY_MISSIONS, 1);
                    break;
                case QuestCategory.ARCHIVEMENT:
                    OnAchivement();
                    break;
            }
        }
        private void HideObj()
        {
            scrollDaily.gameObject.SetActive(false);
            scrollAchievements.gameObject.SetActive(false);
            dailyButton.image.sprite = iconBg[0];
            weeklyButton.image.sprite = iconBg[0];
            achievementsButton.image.sprite = iconBg[0];
            scrollDaily.verticalNormalizedPosition = 1;
            scrollAchievements.verticalNormalizedPosition = 1;
        }
    }
    public enum QuestCategory
    {
        DAILY, WEEKLY, ARCHIVEMENT
    }
}
