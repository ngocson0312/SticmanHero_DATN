using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ChallengeManager : Singleton<ChallengeManager>
    {
        public static event System.Action<bool> OnTaskUpdate;
        public int dayCount
        {
            get { return PlayerPrefs.GetInt("challenge_current_day", 0); }
            set { PlayerPrefs.SetInt("challenge_current_day", value); }
        }

        public int rewardIndex
        {
            get { return PlayerPrefs.GetInt("challenge_reward_idx", 0); }
            set { PlayerPrefs.SetInt("challenge_reward_idx", value); }
        }
        public List<DayChallengeData> dayChallengeDatas;
        public List<QuestGeneratorSO> challengeDaySO;
        public void Initialize()
        {
            dayChallengeDatas = new List<DayChallengeData>();
            LoadData();
            GameManager.resetDay += NextDay;
        }

        public void LoadData()
        {
            if (PlayerPrefs.GetString("challenge_day" + 1, "").Equals(""))
            {
                for (int i = 1; i <= challengeDaySO.Count; i++)
                {
                    DayChallengeData dayChallengeData = new DayChallengeData();
                    dayChallengeData.listDayChallenge = challengeDaySO[i - 1].QuestGenerator();
                    dayChallengeDatas.Add(dayChallengeData);
                    string data = JsonUtility.ToJson(dayChallengeDatas[i - 1]);
                    PlayerPrefs.SetString("challenge_day" + i, data);
                }
            }
            else
            {
                for (int i = 1; i <= challengeDaySO.Count; i++)
                {
                    dayChallengeDatas.Add(JsonUtility.FromJson<DayChallengeData>(PlayerPrefs.GetString("challenge_day" + i, "")));
                }
            }
        }

        public void UpdateTask(QuestType questType, int amount)
        {
            bool complete = false;
            for (int i = 0; i < dayCount; i++)
            {
                for (int j = 0; j < dayChallengeDatas[i].listDayChallenge.Count; j++)
                {
                    if (dayChallengeDatas[i].listDayChallenge[j].questType == questType)
                    {
                        dayChallengeDatas[i].listDayChallenge[j].current += amount;
                        if (dayChallengeDatas[i].listDayChallenge[j].current >= dayChallengeDatas[i].listDayChallenge[j].require && !dayChallengeDatas[i].listDayChallenge[j].claimed)
                        {
                            complete = true;
                        }
                    }
                }
            }

            OnTaskUpdate?.Invoke(complete);
            SaveData();
        }
        public void SaveData()
        {
            for (int i = 1; i <= dayCount; i++)
            {
                SaveDataDay(i);
            }
        }
        public void SaveDataDay(int i)
        {
            string data = JsonUtility.ToJson(dayChallengeDatas[i-1]);
            PlayerPrefs.SetString("challenge_day" + i, data);
        }

        public void ResetDay()
        {
            GameManager.resetDay += NextDay;
        }

        void NextDay()
        {
            if (dayCount < 7)
            {
                dayCount++;
            } else
            {
                dayCount = 7; 
            }
        }

    }

    [System.Serializable]
    public class DayChallengeData
    {
        public DayChallengeData()
        {
            listDayChallenge = new List<QuestData>();
        }
        public List<QuestData> listDayChallenge;
    }
}
