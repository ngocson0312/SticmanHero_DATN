using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace SuperFight
{
    [CreateAssetMenu(fileName = "QuestGenerator", menuName = "Data/QuestGeneratorSO", order = 0)]
    public class QuestGeneratorSO : ScriptableObject
    {
        public int numQuest;
        public QuestSample[] questSamples;
        public List<QuestData> QuestGenerator()
        {
            List<QuestSample> listSample = questSamples.ToList();
            List<QuestData> listQuest = new List<QuestData>();
            for (int i = 0; i < numQuest; i++)
            {
                QuestData questData = new QuestData();
                QuestSample sample = GetRandomQuest();
                questData.questType = sample.questType;
                questData.require = sample.require;
                questData.gemReward = sample.gemReward;
                questData.coinReward = sample.coinReward;
                questData.pointMission = sample.pointMission;
                questData.description = sample.description;
                questData.increase = sample.increase;
                listQuest.Add(questData);
            }
            QuestSample GetRandomQuest()
            {
                int r = Random.Range(0, listSample.Count);
                QuestSample sample = listSample[r];
                listSample.RemoveAt(r);
                return sample;
            }
            return listQuest;
        }
    }
    [System.Serializable]
    public class QuestSample
    {
        public QuestType questType;
        public int require;
        public string description;
        public int coinReward;
        public int gemReward;
        public int pointMission;
        public int increase;
    }
}
