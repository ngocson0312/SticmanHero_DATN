using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class PreviewReward : MonoBehaviour
    {
        public List<RewardItemUI> rewardItemUIs;
        public List<ItemRewardMission> itemRewardMissions;
        public List<Sprite> iconItem;
        public void SetInfo()
        {
            for (int i = 0; i < itemRewardMissions.Count; i++)
            {
                if (itemRewardMissions[i].type == TypeGift.Coin)
                {
                    rewardItemUIs[i].iconImg.sprite = iconItem[0];
                }
                else if (itemRewardMissions[i].type == TypeGift.Diamond)
                {
                    rewardItemUIs[i].iconImg.sprite = iconItem[1];
                }
                else
                {
                    EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemRewardMissions[i].equipmentData.itemID);
                    rewardItemUIs[i].iconImg.sprite = equipmentObject.icon;
                }
            }
        }

        public void Claim()
        {
            for (int i = 0; i < itemRewardMissions.Count; i++)
            {
                if (itemRewardMissions[i].type == TypeGift.Coin)
                {
                    DataManager.Instance.AddCoin(itemRewardMissions[i].amout, 0, "RewardChest");
                }
                else if (itemRewardMissions[i].type == TypeGift.Diamond)
                {
                    DataManager.Instance.AddDiamond(itemRewardMissions[i].amout, 0, "RewardChest");
                }
                else
                {
                    Inventory.Instance.AddItem(itemRewardMissions[i].equipmentData);
                }
            }
        }

    }


}
