using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class ChallengeDay : MonoBehaviour
    {
        public QuestTaskButton buttonPrefab;
        public List<QuestTaskButton> listButton;
        private List<QuestData> questDatas;
        public Transform holder;
        public GameObject lockIcon;
        public int total;
        private ChallengePopup challengePopup;
        public ScrollRect scrollRect;
        public void Initialize(int idx, ChallengePopup challengePopup)
        {
            this.challengePopup = challengePopup;
            this.questDatas = ChallengeManager.Instance.dayChallengeDatas[idx].listDayChallenge;
            listButton = new List<QuestTaskButton>();
            for (int i = 0; i < this.questDatas.Count; i++)
            {
                buttonPrefab.UpdateTask(this.questDatas[i]);
                QuestTaskButton button = Instantiate(buttonPrefab, holder);
                button.Initialize();
                button.OnClaim += OnClaimMission;
                listButton.Add(button);
            }
            lockIcon.SetActive(false);
        }
        public void UpdateChallenge()
        {
            total = 0;
            for (int i = 0; i < listButton.Count; i++)
            {
                if (i < questDatas.Count)
                {
                    listButton[i].UpdateTask(questDatas[i]);
                    if (questDatas[i].claimed)
                    {
                        total += questDatas[i].pointMission;
                    }
                }
                else
                {
                    listButton[i].UpdateTask(null);
                }
            }
            for (int i = 0; i < holder.childCount; i++)
            {
                QuestTaskButton questTaskButton = holder.GetChild(i).GetComponent<QuestTaskButton>();
                if (questTaskButton.questData.current >= questTaskButton.questData.require)
                {
                    scrollRect.content.GetChild(i).SetSiblingIndex(0);
                }
            }
        }

        private void OnClaimMission(QuestData questData)
        {

            challengePopup.SetTotal();
        }
    }
}
