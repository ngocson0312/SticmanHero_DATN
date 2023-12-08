using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class QuestManager : Singleton<QuestManager>
    {
        public static event Action<int[], bool> OnTaskUpdate;
        private string dailyQuestDataPref
        {
            get { return PlayerPrefs.GetString("sf_daily_quest_pref", ""); }
            set { PlayerPrefs.SetString("sf_daily_quest_pref", value); }
        }
        private string weeklyQuestDataPref
        {
            get { return PlayerPrefs.GetString("sf_weekly_quest_pref", ""); }
            set { PlayerPrefs.SetString("sf_weekly_quest_pref", value); }
        }
        private string achievementQuestDataPref
        {
            get { return PlayerPrefs.GetString("sf_achievement_quest_pref", ""); }
            set { PlayerPrefs.SetString("sf_achievement_quest_pref", value); }
        }

        private string Yesterday
        {
            get { return PlayerPrefs.GetString("daily_quest_time", ""); }
            set { PlayerPrefs.SetString("daily_quest_time", value); }
        }
        private string LastWeek
        {
            get { return PlayerPrefs.GetString("weekly_quest_time", ""); }
            set { PlayerPrefs.SetString("weekly_quest_time", value); }
        }
        public QuestGeneratorSO dailyQuestGenerator;
        public QuestGeneratorSO weeklyQuestGenerator;
        public QuestGeneratorSO achievementsQuestGenerator;
        public DailyQuest dailyQuestObj;
        public WeeklyQuest weeklyQuestObj;
        public AchievementsQuest achievementsQuestObj;
        public void Initialize()
        {
            if (Yesterday.Length > 0)
            {
                DateTime dateTime = DateTime.Parse(Yesterday);
                if (dateTime < DateTime.Today)
                {
                    dailyQuestObj = new DailyQuest();
                    dailyQuestObj.listQuest = dailyQuestGenerator.QuestGenerator();
                    Yesterday = DateTime.Today.ToString();
                    SaveDaiLy();
                }
                else
                {
                    dailyQuestObj = JsonUtility.FromJson<DailyQuest>(dailyQuestDataPref);
                }
            }
            else
            {
                dailyQuestObj = new DailyQuest();
                dailyQuestObj.listQuest = dailyQuestGenerator.QuestGenerator();
                Yesterday = DateTime.Today.ToString();
                SaveDaiLy();
            }
            if (LastWeek.Length > 0)
            {
                DateTime lastWeek = DateTime.Parse(LastWeek);
                int nextDiff = 7 - (int)lastWeek.DayOfWeek;
                TimeSpan diff = DateTime.Today - lastWeek;
                if (diff.TotalDays >= nextDiff)
                {
                    weeklyQuestObj = new WeeklyQuest();
                    weeklyQuestObj.listQuest = weeklyQuestGenerator.QuestGenerator();
                    LastWeek = DateTime.Today.ToString();
                    SaveWeekly();
                }
                else
                {
                    weeklyQuestObj = JsonUtility.FromJson<WeeklyQuest>(weeklyQuestDataPref);
                }
            }
            else
            {
                weeklyQuestObj = new WeeklyQuest();
                weeklyQuestObj.listQuest = weeklyQuestGenerator.QuestGenerator();
                LastWeek = DateTime.Today.ToString();
                SaveWeekly();
            }

            if (achievementQuestDataPref.Equals("") || achievementQuestDataPref.Length < 0)
            {
                achievementsQuestObj = new AchievementsQuest();
                achievementsQuestObj.listQuest = achievementsQuestGenerator.QuestGenerator();
                SaveAchievements();
            }
            else
            {
                achievementsQuestObj = JsonUtility.FromJson<AchievementsQuest>(achievementQuestDataPref);
            }
            UpdateTask(QuestType.SPEND_COIN, 0);
        }
        public void UpdateTask(QuestType questType, int amount)
        {
            int[] types = new int[3];
            bool complete = false;
            for (int i = 0; i < dailyQuestObj.listQuest.Count; i++)
            {
                if (dailyQuestObj.listQuest[i].questType == questType)
                {
                    dailyQuestObj.listQuest[i].current += amount;
                }
                if (dailyQuestObj.listQuest[i].current >= dailyQuestObj.listQuest[i].require && !dailyQuestObj.listQuest[i].claimed)
                {
                    types[0] = 1;
                    complete = true;
                }
            }

            for (int i = 0; i < weeklyQuestObj.listQuest.Count; i++)
            {
                if (weeklyQuestObj.listQuest[i].questType == questType)
                {
                    weeklyQuestObj.listQuest[i].current += amount;
                }
                if (weeklyQuestObj.listQuest[i].current >= weeklyQuestObj.listQuest[i].require && !weeklyQuestObj.listQuest[i].claimed)
                {
                    types[1] = 1;
                    complete = true;
                }
            }

            for (int i = 0; i < achievementsQuestObj.listQuest.Count; i++)
            {
                if (achievementsQuestObj.listQuest[i].questType == questType)
                {
                    achievementsQuestObj.listQuest[i].current += amount;
                }
                if (achievementsQuestObj.listQuest[i].current >= achievementsQuestObj.listQuest[i].require && !achievementsQuestObj.listQuest[i].claimed)
                {
                    types[2] = 1;
                    complete = true;
                }
            }
            OnTaskUpdate?.Invoke(types, complete);
            SaveData();
        }
        public void GenerateQuest()
        {
            dailyQuestObj = new DailyQuest();
            dailyQuestObj.listQuest = dailyQuestGenerator.QuestGenerator();

            weeklyQuestObj = new WeeklyQuest();
            weeklyQuestObj.listQuest = weeklyQuestGenerator.QuestGenerator();

            achievementsQuestObj = new AchievementsQuest();
            achievementsQuestObj.listQuest = achievementsQuestGenerator.QuestGenerator();

            SaveData();
        }
        public void SaveData()
        {
            SaveDaiLy();
            SaveWeekly();
            SaveAchievements();
        }

        public void SaveDaiLy()
        {
            string data = JsonUtility.ToJson(dailyQuestObj);
            dailyQuestDataPref = data;
        }

        public void SaveWeekly()
        {
            string data = JsonUtility.ToJson(weeklyQuestObj);
            weeklyQuestDataPref = data;
        }

        public void SaveAchievements()
        {
            string data = JsonUtility.ToJson(achievementsQuestObj);
            achievementQuestDataPref = data;
        }
    }
    [System.Serializable]
    public class DailyQuest
    {
        public DailyQuest()
        {
            listQuest = new List<QuestData>();
            rewardIndex = 0;
        }
        public int rewardIndex;
        public List<QuestData> listQuest;
    }

    [System.Serializable]
    public class WeeklyQuest
    {
        public WeeklyQuest()
        {
            listQuest = new List<QuestData>();
            rewardIndex = 0;
        }
        public List<QuestData> listQuest;
        public int rewardIndex;
    }
    [System.Serializable]
    public class AchievementsQuest
    {
        public AchievementsQuest()
        {
            listQuest = new List<QuestData>();
        }
        public List<QuestData> listQuest;
    }
    [System.Serializable]
    public class QuestData
    {
        public QuestData()
        {
            gemReward = 0;
            coinReward = 0;
            current = 0;
        }
        public string description;
        public int require;
        public int current;
        public int coinReward;
        public int gemReward;
        public bool claimed;
        public int pointMission;
        public int increase;
        public QuestType questType;
    }
    public enum QuestType
    {
        LOGIN, OPEN_CHEST, SPEND_DIAMOND, SPEND_COIN, UPGRADE_EQUIPMENT, KILL_ENEMY, CLEAR_LEVEL, CLEAR_CHAPTER,
        EARN_COIN, EARN_DIAMOND, MERGE_EQUIPMENT, COMPLETE_DAILY_MISSIONS, REVIVE_INGAME, COMPLETE_WEEKLY_MISSIONS,
        UPGRADE_WEAPON, UPGRADE_ARMOR, UPGRADE_BOOTS, UPGRADE_RING, UPGRADE_NECKLACE, KILL_BOSS
    }
}
