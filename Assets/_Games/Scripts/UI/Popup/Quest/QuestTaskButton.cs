using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class QuestTaskButton : MonoBehaviour
    {
        public QuestData questData;
        [SerializeField] Button button;
        [SerializeField] Image backGroundImg;
        [SerializeField] Text descriptionText;
        [SerializeField] Image progressFill;
        [SerializeField] Text rewardText;
        [SerializeField] GameObject[] icons;
        [SerializeField] Sprite[] sprs;
        [SerializeField] Text progressText;
        [SerializeField] Text pointMissionText;
        public System.Action<QuestData> OnClaim;
        public void Initialize()
        {
            button.onClick.AddListener(ClaimReward);
        }
        public void UpdateTask(QuestData questData)
        {
            this.questData = questData;
            if (questData == null || questData.claimed)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (questData.current >= questData.require)
            {
                button.image.sprite = sprs[0];
                // backGroundImg.color = new Color(1f, 1f, 0.5f, 1f);
                // button.interactable = true;
            }
            else
            {
                button.image.sprite = sprs[1];
                // backGroundImg.color = new Color(1, 1, 1, 0.7f);
                // button.interactable = false;
            }
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetActive(false);
            }
            if (questData.coinReward > 0)
            {
                icons[0].SetActive(true);
                rewardText.text = questData.coinReward.ToString();
            }
            else
            {
                icons[1].SetActive(true);
                rewardText.text = questData.gemReward.ToString();
            }
            string des = questData.description;
            des = des.Replace("0", TopUI.ConvertMoneyToString(questData.require));
            descriptionText.text = des;
            float fillAmount = (float)questData.current / (float)questData.require;
            progressFill.fillAmount = fillAmount;
            progressText.text = TopUI.ConvertMoneyToString(questData.current) + " / " + TopUI.ConvertMoneyToString(questData.require);
            pointMissionText.text = $"{questData.pointMission}";
        }
        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }
        private void ClaimReward()
        {
            if (questData.claimed == true || questData.current < questData.require) return;
            AudioManager.Instance.PlayOneShot("eff_coin_ui", 1f);
            questData.claimed = true;
            DataManager.Instance.AddDiamond(questData.gemReward, 0, "quest");
            DataManager.Instance.AddCoin(questData.coinReward, 0, "quest");
            if (questData.increase > 0 && questData.gemReward * questData.increase < 3000)
            {
                questData.require *= questData.increase;
                questData.gemReward *= questData.increase;
                questData.claimed = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
            // QuestManager.Instance.UpdateTask(QuestType.COMPLETE_QUEST, 1);
            QuestManager.Instance.SaveData();
            // ChallengeManager.Instance.SaveData();
            OnClaim?.Invoke(questData);
        }
    }
}
