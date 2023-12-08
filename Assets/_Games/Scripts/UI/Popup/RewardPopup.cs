using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class RewardPopup : PopupUI
    {
        [SerializeField] Button closeBtn;
        [SerializeField] RewardItemUI rewardItemPrefab;
        [SerializeField] Transform holder;
        [SerializeField] ItemUtilitiesSO itemUtilities;
        private List<RewardItemUI> listItem;
        private RewardInfo[] rewardInfos;
        private bool ready;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            closeBtn.onClick.AddListener(Hide);
            listItem = new List<RewardItemUI>();
        }
        public void ShowReward(RewardInfo[] rewardInfos)
        {
            this.rewardInfos = rewardInfos;
            ready = false;
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].gameObject.SetActive(false);
            }
            if (rewardInfos.Length >= listItem.Count)
            {
                for (int i = 0; i < rewardInfos.Length; i++)
                {
                    Sprite rarity = itemUtilities.GetGradeBackGround(rewardInfos[i].rarity);
                    Sprite icon = rewardInfos[i].itemObject.icon;
                    string content = "";
                    if (rewardInfos[i].itemObject.itemType != ItemType.Equiment)
                    {
                        content = "x" + rewardInfos[i].amount;
                    }
                    if (i < listItem.Count)
                    {
                        listItem[i].SetInfo(rarity, icon, content, i);
                    }
                    else
                    {
                        RewardItemUI rewardItem = Instantiate(rewardItemPrefab, holder);
                        rewardItem.SetInfo(rarity, icon, content, i);
                        listItem.Add(rewardItem);
                    }
                }
            }
            else
            {
                for (int i = 0; i < listItem.Count; i++)
                {
                    if (i < rewardInfos.Length)
                    {
                        Sprite rarity = itemUtilities.GetGradeBackGround(rewardInfos[i].rarity);
                        Sprite icon = rewardInfos[i].itemObject.icon;
                        string content = "";
                        if (rewardInfos[i].itemObject.itemType != ItemType.Equiment)
                        {
                            content = "x" + rewardInfos[i].amount;
                        }
                        listItem[i].SetInfo(rarity, icon, content, i);
                    }
                    else
                    {
                        listItem[i].gameObject.SetActive(false);
                    }
                }
            }
            GameManager.Instance.DelayCallBack(rewardInfos.Length * 0.15f + 1f, () =>
            {
                ready = true;
            });
        }
        public override void Hide()
        {
            if (!ready) return;
            for (int i = 0; i < rewardInfos.Length; i++)
            {
                switch (rewardInfos[i].itemObject.itemType)
                {
                    case ItemType.Equiment:
                        Inventory.Instance.AddItem(new EquipmentData(rewardInfos[i].itemObject.id, rewardInfos[i].rarity));
                        break;
                    case ItemType.Coin:
                        DataManager.Instance.AddCoin(rewardInfos[i].amount, 0, "");
                        break;
                    case ItemType.Diamond:
                        DataManager.Instance.AddDiamond(rewardInfos[i].amount, 0, "");
                        break;
                    case ItemType.Useable:
                        if (rewardInfos[i].itemObject is KeyObjectSO)
                        {
                            KeyObjectSO keyObject = rewardInfos[i].itemObject as KeyObjectSO;
                            DataManager.Instance.AddKey(rewardInfos[i].amount, keyObject.typeKey);
                        }
                        break;
                    case ItemType.Material:
                        break;
                }
            }
            base.Hide();
        }
    }
    [System.Serializable]
    public struct RewardInfo
    {
        public ItemObjectSO itemObject;
        public int amount;
        public int rarity;
    }
}
