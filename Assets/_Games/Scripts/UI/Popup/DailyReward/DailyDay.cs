using UnityEngine.UI;
using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
namespace SuperFight
{
    public class DailyDay : MonoBehaviour
    {
        public int day;
        public DailyRewardItem reward;
        public Image backGround;
        public Text textDay;
        public Text textCoin;
        public Sprite[] bgSprs;
        public GameObject checkMark;
        private DailyReward dailyReward;
        public Button button;
        public Image iconReward;
        public List<Sprite> icon;
        public void Initialize(DailyReward dailyReward)
        {
            this.dailyReward = dailyReward;
            textDay.text = "DAY " + day.ToString();
            checkMark.SetActive(false);
            if (dailyReward.currentGift == day)
            {
                if (this.dailyReward.canGetGift <= 0)
                {
 
                    backGround.sprite = bgSprs[2];
                    checkMark.SetActive(true);
                }
                else
                {
                    backGround.sprite = bgSprs[0];
                    button.onClick.AddListener(ClaimGift);
                }
            }
            if (dailyReward.currentGift < day)
            {
                backGround.sprite = bgSprs[1];
            }
            if (dailyReward.currentGift > day)
            {
                checkMark.SetActive(true);
                backGround.sprite = bgSprs[2];
            }

            if (reward.coin > 0 || reward.diamond > 0)
            {
                textCoin.gameObject.SetActive(true);
                textCoin.text = "+ " + ( reward.coin > 0 ? reward.coin.ToString() : reward.diamond.ToString());
                iconReward.sprite = (reward.coin > 0 ? icon[1] : icon[0]);
            } else
            {
                
                EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(reward.equipmentData.itemID);
                iconReward.sprite = equipmentObject.icon;
                textCoin.text = equipmentObject.name;
            }
        }
        public void ClaimGift()
        {
            dailyReward.OnClaim(reward);
            checkMark.SetActive(true);
            backGround.sprite = bgSprs[2];
            checkMark.SetActive(true);
        }
    }

}
